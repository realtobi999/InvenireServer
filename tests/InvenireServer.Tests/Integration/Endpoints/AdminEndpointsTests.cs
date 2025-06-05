using System.Net.Http.Json;
using System.Text.RegularExpressions;
using InvenireServer.Application.Interfaces.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation;
using InvenireServer.Tests.Integration.Extensions;
using InvenireServer.Tests.Integration.Fakers;
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
    public async Task RegisterAdmin_Returns201AndAdminIsCreated()
    {
        // Prepare.
        var admin = new AdminFaker().Generate();

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/auth/admin/register", admin.ToRegisterAdminDto());
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await using var context = _app.GetDatabaseContext();
        var createdAdmin = await context.Admins.FirstOrDefaultAsync(e => e.Id == admin.Id);

        // Assert that the admin is created in the database.
        createdAdmin.Should().NotBeNull();
        // Assert that the password is hashed.
        createdAdmin!.Password.Should().NotBe(admin.Password);
    }

    [Fact]
    public async Task SendEmailVerification_Returns204AndEmailIsSentWithTheACorrectLink()
    {
        // Prepare.
        var admin = new AdminFaker().Generate();

        var jwt = _jwt.Builder.Build([
            new("role", Jwt.Roles.ADMIN),
            new("admin_id", admin.Id.ToString()),
            new("is_verified", bool.FalseString)
        ]);
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(jwt)}");

        (await _client.PostAsJsonAsync("/api/auth/admin/register", admin.ToRegisterAdminDto())).StatusCode.Should().Be(HttpStatusCode.Created);

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
}
