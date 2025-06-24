using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.RegularExpressions;
using InvenireServer.Application.Core.Admins.Commands.Login;
using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Application.Interfaces.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation;
using InvenireServer.Tests.Integration.Extensions;
using InvenireServer.Tests.Integration.Extensions.Users;
using InvenireServer.Tests.Integration.Fakers.Common;
using InvenireServer.Tests.Integration.Fakers.Users;
using InvenireServer.Tests.Integration.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InvenireServer.Tests.Integration.Endpoints;

public class AdminEndpointsTests
{
    private readonly HttpClient _client;
    private readonly IJwtManager _jwt;
    private readonly ServerFactory<Program> _app;

    public AdminEndpointsTests()
    {
        _app = new ServerFactory<Program>();
        _jwt = new JwtManagerFaker().Initiate();
        _client = _app.CreateDefaultClient();
    }

    [Fact]
    public async Task Register_Returns201AndAdminIsCreated()
    {
        // Prepare.
        var admin = new AdminFaker().Generate();

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/auth/admin/register", admin.ToRegisterAdminCommand());
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Assert that the token has all the necessary claims.
        var jwt = JwtBuilder.Parse(await response.Content.ReadAsStringAsync());
        jwt.Payload.Should().Contain(c => c.Type == "role" && c.Value == Jwt.Roles.ADMIN);
        jwt.Payload.Should().Contain(c => c.Type == "admin_id" && c.Value == admin.Id.ToString());
        jwt.Payload.Should().Contain(c => c.Type == "is_verified" && c.Value == bool.FalseString);

        await using var context = _app.GetDatabaseContext();
        var createdAdmin = await context.Admins.FirstOrDefaultAsync(e => e.Id == admin.Id);

        // Assert that the admin is created in the database.
        createdAdmin.Should().NotBeNull();
        // Assert that the password is hashed.
        createdAdmin!.Password.Should().NotBe(admin.Password);
    }

    [Fact]
    public async Task SendVerification_Returns204AndEmailIsSentWithTheACorrectLink()
    {
        // Prepare.
        var admin = new AdminFaker().Generate();

        var jwt = _jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.FalseString)
        ]);
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(jwt)}");

        (await _client.PostAsJsonAsync("/api/auth/admin/register", admin.ToRegisterAdminCommand())).StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/auth/admin/email-verification/send", new object());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var email = (EmailSenderFaker)_app.Services.GetRequiredService<IEmailSender>();
        email.CapturedMessages.Count.Should().Be(1);
        var message = email.CapturedMessages[0];

        // Assert that the email message is properly constructed and contains a verification link.
        message.To.Should().ContainSingle(t => t.Address == admin.EmailAddress);
        message.Subject.Should().Contain("verify your email");
        message.Body.Should().NotBeNull();
        message.Body.Should().MatchRegex(@"https?:\/\/[^\/]+\/verify-email\?token=([\w\-_.]+)");

        var match = Regex.Match(message.Body, @"https?:\/\/[^\/]+\/verify-email\?token=([\w\-_.]+)");
        var token = JwtBuilder.Parse(match.Groups[1].Value);

        // Assert that the token in the verification link contains all important claims.
        token.Payload.Should().Contain(c => c.Type == "role" && c.Value == nameof(Admin).ToUpper());
        token.Payload.Should().Contain(c => c.Type == "admin_id" && c.Value == admin.Id.ToString());
        token.Payload.Should().Contain(c => c.Type == "is_verified" && c.Value == bool.FalseString);
        token.Payload.Should().Contain(c => c.Type == "purpose" && c.Value == "email_verification");
    }

    [Fact]
    public async Task ConfirmVerification_Returns204AndEmailIsVerified()
    {
        // Prepare.
        var admin = new AdminFaker().Generate();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.FalseString)
        ]))}");

        (await _client.PostAsJsonAsync("/api/auth/admin/register", admin.ToRegisterAdminCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/auth/admin/email-verification/send", new object())).StatusCode.Should().Be(HttpStatusCode.NoContent);

        var jwt = _jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.FalseString),
            new Claim("purpose", "email_verification"),
        ]);
        _client.DefaultRequestHeaders.Remove("Authorization");
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(jwt)}");

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/auth/admin/email-verification/confirm", new object());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await using var context = _app.GetDatabaseContext();
        var updatedAdmin = await context.Admins.FirstOrDefaultAsync(e => e.Id == admin.Id);

        // Assert that the admin has a verified email.
        updatedAdmin!.IsVerified.Should().Be(true);
    }

    [Fact]
    public async Task Login_Returns200AndJwtIsReturned()
    {
        // Prepare.
        var admin = new AdminFaker().Generate();

        (await _client.PostAsJsonAsync("/api/auth/admin/register", admin.ToRegisterAdminCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.FalseString),
            new Claim("purpose", "email_verification")
        ]))}");
        (await _client.PostAsJsonAsync("/api/auth/admin/email-verification/confirm", new object())).StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act & Assert.
        var dto = new LoginAdminCommand
        {
            EmailAddress = admin.EmailAddress,
            Password = admin.Password
        };

        var response = await _client.PostAsJsonAsync("/api/auth/admin/login", dto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert that the token has all the necessary claims.
        var jwt = JwtBuilder.Parse(await response.Content.ReadAsStringAsync());
        jwt.Payload.Should().Contain(c => c.Type == "role" && c.Value == Jwt.Roles.ADMIN);
        jwt.Payload.Should().Contain(c => c.Type == "admin_id" && c.Value == admin.Id.ToString());
        jwt.Payload.Should().Contain(c => c.Type == "is_verified" && c.Value == bool.TrueString);
    }

    [Fact]
    public async Task Login_Returns401WhenBadCredentials()
    {
        // Prepare.
        var admin = new AdminFaker().Generate();

        (await _client.PostAsJsonAsync("/api/auth/admin/register", admin.ToRegisterAdminCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.FalseString),
            new Claim("purpose", "email_verification")
        ]))}");
        (await _client.PostAsJsonAsync("/api/auth/admin/email-verification/confirm", new object())).StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act & Assert.
        var dto = new LoginAdminCommand
        {
            EmailAddress = admin.EmailAddress,
            Password = new Faker().Internet.SecurePassword() // Generate a random password.
        };

        var response = await _client.PostAsJsonAsync("/api/auth/admin/login", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_Returns429WhenTooManyRequestsHasBeenSent()
    {
        // Prepare.
        var admin = new AdminFaker().Generate();

        (await _client.PostAsJsonAsync("/api/auth/admin/register", admin.ToRegisterAdminCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.FalseString),
            new Claim("purpose", "email_verification")
        ]))}");
        (await _client.PostAsJsonAsync("/api/auth/admin/email-verification/confirm", new object())).StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Trigger the endpoints 5 times to enable the rare limiter.
        for (var i = 0; i < 5; i++)
            (await _client.PostAsJsonAsync("/api/auth/admin/login", new LoginAdminCommand
            {
                EmailAddress = admin.EmailAddress,
                Password = new Faker().Internet.SecurePassword() // Generate a random password.
            })).StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // Act & Assert.
        var dto = new LoginAdminCommand
        {
            EmailAddress = admin.EmailAddress,
            Password = admin.Password
        };

        var response = await _client.PostAsJsonAsync("/api/auth/admin/login", dto);
        response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }
}