using System.Net.Http.Json;
using System.Security.Claims;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Presentation;
using InvenireServer.Tests.Integration.Extensions.Organizations;
using InvenireServer.Tests.Integration.Extensions.Properties;
using InvenireServer.Tests.Integration.Extensions.Users;
using InvenireServer.Tests.Integration.Fakers.Common;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Properties;
using InvenireServer.Tests.Integration.Fakers.Users;
using InvenireServer.Tests.Integration.Server;

namespace InvenireServer.Tests.Integration.Endpoints;

public class PropertyEndpointsTests
{
    private readonly HttpClient _client;
    private readonly IJwtManager _jwt;
    private readonly ServerFactory<Program> _app;

    public PropertyEndpointsTests()
    {
        _app = new ServerFactory<Program>();
        _jwt = new JwtManagerFaker().Initiate();
        _client = _app.CreateDefaultClient();
    }

    [Fact]
    public async Task Create_ReturnsCreated()
    {
        // prepare.
        var organization = new OrganizationFaker().Generate();
        var property = new PropertyFaker(organization).Generate();
        var admin = new AdminFaker(organization).Generate();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        (await _client.PostAsJsonAsync("/api/auth/admin/register", admin.ToRegisterAdminCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/organizations", organization.ToCreateOrganizationCommand())).StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var response = await _client.PostAsJsonAsync($"/api/organizations/{organization.Id}/properties", property.ToCreatePropertyCommand());
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
