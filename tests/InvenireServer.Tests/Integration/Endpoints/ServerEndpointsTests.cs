using System.Security.Claims;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Presentation;
using InvenireServer.Tests.Fakers.Common;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Integration.Server;

namespace InvenireServer.Tests.Integration.Endpoints;

/// <summary>
/// Integration tests for server utility endpoints.
/// </summary>
public class ServerEndpointsTests
{
    private readonly ServerFactory<Program> _app;
    private readonly HttpClient _client;
    private readonly IJwtManager _jwt;

    public ServerEndpointsTests()
    {
        _app = new ServerFactory<Program>();
        _jwt = JwtManagerFaker.Initiate();
        _client = _app.CreateDefaultClient();
    }

    /// <summary>
    /// Verifies that the health check endpoint returns OK.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        // Act & Assert.
        var response = await _client.GetAsync("/api/server/health-check");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>
    /// Verifies that the auth check endpoint returns OK with a token and Unauthorized without one.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task AuthCheck_ReturnsOkAndUnauthorized()
    {
        // Prepare.
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([]))}");

        // Act & Assert.
        var response1 = await _client.GetAsync("/api/server/auth-check");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);

        // Remove the authorization header and assume the authorization is handled.
        _client.DefaultRequestHeaders.Remove("Authorization");

        var response2 = await _client.GetAsync("/api/server/auth-check");
        response2.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// Verifies that the auth role endpoint returns OK and the expected role.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task GetRole_ReturnsOkAndCorrectData()
    {
        // Prepare.
        var admin = AdminFaker.Fake();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act & Assert.
        var response = await _client.GetAsync("/api/server/auth/role");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        content.Should().Be(Jwt.Roles.ADMIN);
    }
}
