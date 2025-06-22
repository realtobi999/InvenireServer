using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Presentation;
using InvenireServer.Tests.Integration.Fakers;
using InvenireServer.Tests.Integration.Fakers.Common;
using InvenireServer.Tests.Integration.Server;

namespace InvenireServer.Tests.Integration.Endpoints;

public class ServerEndpointsTests
{
    private readonly ServerFactory<Program> _app;
    private readonly HttpClient _client;
    private readonly IJwtManager _jwt;

    public ServerEndpointsTests()
    {
        _app = new ServerFactory<Program>();
        _jwt = new JwtManagerFaker().Initiate();
        _client = _app.CreateDefaultClient();
    }

    [Fact]
    public async Task HealthCheck_ReturnsStatusCode200()
    {
        // Act & Assert.
        var response = await _client.GetAsync("/api/server/health-check");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AuthCheck_ReturnsCorrectStatusCodes()
    {
        // Prepare.
        var jwt = _jwt.Builder.Build([]);
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(jwt)}");

        // Act & Assert.
        var response1 = await _client.GetAsync("/api/server/auth-check");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);

        // Remove the authorization header and assume the authorization is handled.
        _client.DefaultRequestHeaders.Remove("Authorization");

        var response2 = await _client.GetAsync("/api/server/auth-check");
        response2.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}