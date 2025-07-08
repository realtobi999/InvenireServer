using System.Net.Http.Json;
using System.Security.Claims;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Application.Core.Properties.Items.Commands.Update;
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

        (await _client.PostAsJsonAsync("/api/admins/register", admin.ToRegisterAdminCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        admin.SetAsVerified(_app.GetDatabaseContext());
        (await _client.PostAsJsonAsync("/api/organizations", organization.ToCreateOrganizationCommand())).StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/properties", property.ToCreatePropertyCommand());
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

        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(new PropertyItemFaker(property, employee).Generate());

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        (await _client.PostAsJsonAsync("/api/admins/register", admin.ToRegisterAdminCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/employees/register", employee.ToRegisterEmployeeCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        admin.SetAsVerified(_app.GetDatabaseContext());
        employee.SetAsVerified(_app.GetDatabaseContext());
        (await _client.PostAsJsonAsync("/api/organizations", organization.ToCreateOrganizationCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/properties", property.ToCreatePropertyCommand())).StatusCode.Should().Be(HttpStatusCode.Created);

        organization.AddEmployee(employee, _app.GetDatabaseContext()); // Assign the employee to the organization.

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/properties/items", new CreatePropertyItemsCommand
        {
            Items = [.. items.Select(i => i.ToCreatePropertyItemCommand())]
        });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task UpdateItems_ReturnsNoContent()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var admin = new AdminFaker(organization).Generate();
        var employee1 = new EmployeeFaker().Generate();
        var employee2 = new EmployeeFaker().Generate();
        var property = new PropertyFaker(organization).Generate();

        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(new PropertyItemFaker(property, employee1).Generate());

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        (await _client.PostAsJsonAsync("/api/admins/register", admin.ToRegisterAdminCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/employees/register", employee1.ToRegisterEmployeeCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/employees/register", employee2.ToRegisterEmployeeCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        admin.SetAsVerified(_app.GetDatabaseContext());
        employee1.SetAsVerified(_app.GetDatabaseContext());
        employee2.SetAsVerified(_app.GetDatabaseContext());
        (await _client.PostAsJsonAsync("/api/organizations", organization.ToCreateOrganizationCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/properties", property.ToCreatePropertyCommand())).StatusCode.Should().Be(HttpStatusCode.Created);

        // Assign the employees to the organization.
        organization.AddEmployee(employee1, _app.GetDatabaseContext());
        organization.AddEmployee(employee2, _app.GetDatabaseContext());

        (await _client.PostAsJsonAsync("/api/properties/items", new CreatePropertyItemsCommand
        {
            Items = [.. items.Select(i => i.ToCreatePropertyItemCommand())]
        })).StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var response = await _client.PutAsJsonAsync("/api/properties/items", new UpdatePropertyItemsCommand
        {
            Items =
            [
                .. items.Select(i => new UpdatePropertyItemCommand
                {
                    Id = i.Id,
                    InventoryNumber = Guid.NewGuid().ToString(),
                    RegistrationNumber = Guid.NewGuid().ToString(),
                    Name = "TEST",
                    Price = 999,
                    SerialNumber = Guid.NewGuid().ToString(),
                    DateOfPurchase = DateTimeOffset.UtcNow.AddYears(-5),
                    DateOfSale = DateTimeOffset.Now.AddYears(-3),
                    Description = "TEST",
                    DocumentNumber = Guid.NewGuid().ToString(),
                    EmployeeId = new Random().NextDouble() < 0.5 ? employee2.Id : null // 50% chance of changing the employee or removing it.
                })
            ]
        });
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteItems_ReturnsNoContent()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var admin = new AdminFaker(organization).Generate();
        var employee = new EmployeeFaker().Generate();
        var property = new PropertyFaker(organization).Generate();

        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(new PropertyItemFaker(property, employee).Generate());

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        (await _client.PostAsJsonAsync("/api/admins/register", admin.ToRegisterAdminCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/employees/register", employee.ToRegisterEmployeeCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        admin.SetAsVerified(_app.GetDatabaseContext());
        employee.SetAsVerified(_app.GetDatabaseContext());
        (await _client.PostAsJsonAsync("/api/organizations", organization.ToCreateOrganizationCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/properties", property.ToCreatePropertyCommand())).StatusCode.Should().Be(HttpStatusCode.Created);

        // Assign the employees to the organization.
        organization.AddEmployee(employee, _app.GetDatabaseContext());

        (await _client.PostAsJsonAsync("/api/properties/items", new CreatePropertyItemsCommand
        {
            Items = [.. items.Select(i => i.ToCreatePropertyItemCommand())]
        })).StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var response = await _client.DeleteAsync($"/api/properties/items?ids={string.Join("&ids=", items.Select(i => i.Id))}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}