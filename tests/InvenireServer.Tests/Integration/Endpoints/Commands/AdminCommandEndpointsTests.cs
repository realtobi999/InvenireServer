using System.Net.Http.Json;
using System.Security.Claims;
using InvenireServer.Application.Core.Admins.Commands.Login;
using InvenireServer.Application.Core.Admins.Commands.Update;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation;
using InvenireServer.Tests.Extensions.Users;
using InvenireServer.Tests.Fakers.Common;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Integration.Server;

namespace InvenireServer.Tests.Integration.Endpoints.Commands;

/// <summary>
/// Integration tests for admin command endpoints.
/// </summary>
public class AdminCommandEndpointsTests
{
    private readonly ServerFactory<Program> _app;
    private readonly HttpClient _client;
    private readonly JwtManager _jwt;

    public AdminCommandEndpointsTests()
    {
        _app = new ServerFactory<Program>();
        _jwt = JwtManagerFaker.Initiate();
        _client = _app.CreateDefaultClient();
    }

    /// <summary>
    /// Verifies that the admin registration endpoint returns Created.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Register_ReturnsCreated()
    {
        // Prepare.
        var admin = AdminFaker.Fake();

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/admins/register", admin.ToRegisterAdminCommand());
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    /// <summary>
    /// Verifies that the verification email endpoint returns NoContent for an unverified admin.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task SendVerification_ReturnsNoContent()
    {
        // Prepare.
        var admin = AdminFaker.Fake();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.FalseString)
        ]))}");

        (await _client.PostAsJsonAsync("/api/admins/register", admin.ToRegisterAdminCommand())).StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/admins/email-verification/send", new object());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Verifies that the verification confirmation endpoint returns NoContent with a verification token.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task ConfirmVerification_ReturnsNoContent()
    {
        // Prepare.
        var admin = AdminFaker.Fake();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.FalseString)
        ]))}");

        (await _client.PostAsJsonAsync("/api/admins/register", admin.ToRegisterAdminCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/admins/email-verification/send", new object())).StatusCode.Should().Be(HttpStatusCode.NoContent);

        _client.DefaultRequestHeaders.Remove("Authorization");
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.FalseString),
            new Claim("purpose", "email_verification")
        ]))}");

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/admins/email-verification/confirm", new object());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Verifies that the admin login endpoint returns NoContent for valid credentials.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Login_ReturnsNoContent()
    {
        // Prepare.
        var admin = AdminFaker.Fake();

        (await _client.PostAsJsonAsync("/api/admins/register", admin.ToRegisterAdminCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        admin.SetAsVerified(_app.GetDatabaseContext());

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/admins/login", new LoginAdminCommand
        {
            EmailAddress = admin.EmailAddress,
            Password = admin.Password
        });
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Verifies that the admin update endpoint returns NoContent for an authorized admin.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Update_ReturnsNoContent()
    {
        // Prepare.
        var admin = AdminFaker.Fake();

        (await _client.PostAsJsonAsync("/api/admins/register", admin.ToRegisterAdminCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        admin.SetAsVerified(_app.GetDatabaseContext());

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act & Assert.
        var response = await _client.PutAsJsonAsync("/api/admins", new UpdateAdminCommand
        {
            FirstName = new Faker().Name.FirstName(),
            LastName = new Faker().Name.LastName(),
        });
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Verifies that the admin delete endpoint returns NoContent for an authorized admin.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        // Prepare.
        var admin = AdminFaker.Fake();

        (await _client.PostAsJsonAsync("/api/admins/register", admin.ToRegisterAdminCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        admin.SetAsVerified(_app.GetDatabaseContext());

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act & Assert.
        var response = await _client.DeleteAsync("/api/admins");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
