using System.Net.Http.Json;
using System.Security.Claims;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Infrastructure.Authentication;
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
    private readonly ServerFactory<Program> _app;
    private readonly HttpClient _client;
    private readonly JwtManager _jwt;

    public PropertyEndpointsTests()
    {
        _app = new ServerFactory<Program>();
        _jwt = new JwtManagerFaker().Initiate();
        _client = _app.CreateDefaultClient();
    }

    [Fact]
    public async Task Create_ReturnsCreated()
    {
        // Prepare.
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

    [Fact]
    public async Task CreateItems_ReturnsCreated()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var admin = new AdminFaker(organization).Generate();
        var employee = new EmployeeFaker().Generate();
        var property = new PropertyFaker(organization).Generate();
        var invitation = new OrganizationInvitationFaker(organization, employee).Generate();

        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(new PropertyItemFaker(property, employee).Generate());

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        (await _client.PostAsJsonAsync("/api/auth/admin/register", admin.ToRegisterAdminCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/organizations", organization.ToCreateOrganizationCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync($"/api/organizations/{organization.Id}/properties", property.ToCreatePropertyCommand())).StatusCode.Should().Be(HttpStatusCode.Created);

        organization.AddEmployee(employee, _app.GetDatabaseContext()); // Assign the employee to the organization.

        // Act & Assert.
        var response = await _client.PostAsJsonAsync($"/api/organizations/{organization.Id}/properties/{property.Id}/items", new CreatePropertyItemsCommand
        {
            Items = [.. items.Select(i => i.ToCreatePropertyItemCommand())]
        });
    }
}