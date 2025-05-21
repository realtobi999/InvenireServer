using InvenireServer.Presentation;
using InvenireServer.Tests.Integration.Fakers;
using InvenireServer.Tests.Integration.Server;

namespace InvenireServer.Tests.Integration.Endpoints;

public class ServerEndpointsTests
{
    [Fact]
    public async Task HealthCheck_ReturnsStatusCode200()
    {
        // Prepare.
        var app = new ServerFactory<Program>();
        var client = app.CreateDefaultClient();

        // Act & Assert.
        var response = await client.GetAsync("/api/server/health-check");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AuthCheck_ReturnsCorrectStatusCodes()
    {
        // Prepare.
        var app = new ServerFactory<Program>();
        var client = app.CreateDefaultClient();

        var jwt = JwtFaker.Create([]);
        client.DefaultRequestHeaders.Add("Authorization", $"BEARER {jwt.Write()}");

        // Act & Assert.
        var response1 = await client.GetAsync("/api/server/auth-check");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);

        // Remove the authorization header and assume the authorization is handled.
        client.DefaultRequestHeaders.Remove("Authorization");

        var response2 = await client.GetAsync("/api/server/auth-check");
        response2.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
