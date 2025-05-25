using InvenireServer.Presentation;
using InvenireServer.Tests.Integration.Fakers;
using InvenireServer.Tests.Integration.Server;

namespace InvenireServer.Tests.Integration.Endpoints;

public class ServerEndpointsTests
{
    private readonly HttpClient _client;
    private readonly ServerFactory<Program> _app;

    public ServerEndpointsTests()
    {
        _app = new ServerFactory<Program>();
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
        var jwt = JwtFaker.Create([]);
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {jwt.Write()}");

        // Act & Assert.
        var response1 = await _client.GetAsync("/api/server/auth-check");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);

        // Remove the authorization header and assume the authorization is handled.
        _client.DefaultRequestHeaders.Remove("Authorization");

        var response2 = await _client.GetAsync("/api/server/auth-check");
        response2.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
