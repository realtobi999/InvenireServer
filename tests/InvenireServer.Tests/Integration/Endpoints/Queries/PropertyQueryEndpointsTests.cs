using System.Net.Http.Json;
using System.Security.Claims;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Application.Core.Properties.Items.Commands.Update;
using InvenireServer.Application.Core.Properties.Scans.Queries.IndexForAdmin;
using InvenireServer.Application.Core.Properties.Suggestions.Commands;
using InvenireServer.Application.Dtos.Properties;
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
using InvenireServer.Tests.Integration.Fakers.Properties.Items;
using InvenireServer.Tests.Integration.Fakers.Users;
using InvenireServer.Tests.Integration.Server;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;

namespace InvenireServer.Tests.Integration.Endpoints.Queries;

public class PropertyQueryEndpointsTests
{
    private readonly ServerFactory<Program> _app;
    private readonly HttpClient _client;
    private readonly JwtManager _jwt;

    public PropertyQueryEndpointsTests()
    {
        _app = new ServerFactory<Program>();
        _jwt = JwtManagerFaker.Initiate();
        _client = _app.CreateDefaultClient();
    }

    [Fact]
    public async Task GetByAdmin_ReturnsOkAndCorrectData()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var property = PropertyFaker.Fake();
        var suggestion = PropertySuggestionFaker.Fake();
        var organization = OrganizationFaker.Fake();

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

        // Assign the employee to the organization.
        organization.AddEmployee(employee, _app.GetDatabaseContext());

        (await _client.PostAsJsonAsync("/api/properties/items", new CreatePropertyItemsCommand
        {
            Items = [.. items.Select(i => i.ToCreatePropertyItemCommand())]
        })).StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var response = await _client.GetAsync("/api/properties");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert that the response content is correct.
        var content = await response.Content.ReadFromJsonAsync<PropertyDto>() ?? throw new NullReferenceException();

        content.Id.Should().Be(property.Id);
        content.OrganizationId.Should().Be(organization.Id);
        content.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        content.LastUpdatedAt.Should().BeNull();
        content.ItemsSummary.Should().NotBeNull();
        content.ItemsSummary!.TotalItems.Should().Be(items.Count);
        content.ItemsSummary!.TotalValue.Should().Be(items.Sum(i => i.Price));
        content.ScansSummary.Should().BeNull();
        content.SuggestionsSummary.Should().BeNull();
    }

    [Fact]
    public async Task IndexItemsForAdmin_ReturnsOkAndCorrectData()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 300; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var property = PropertyFaker.Fake();
        var suggestion = PropertySuggestionFaker.Fake();
        var organization = OrganizationFaker.Fake();

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

        // Assign the employee to the organization.
        organization.AddEmployee(employee, _app.GetDatabaseContext());

        (await _client.PostAsJsonAsync("/api/properties/items", new CreatePropertyItemsCommand
        {
            Items = [.. items.Select(i => i.ToCreatePropertyItemCommand())]
        })).StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var limit = 100;
        var offset = 100;
        var response = await _client.GetAsync($"/api/properties/items?limit={limit}&offset={offset}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert that the response content is correct.
        var content = await response.Content.ReadFromJsonAsync<List<PropertyItemDto>>() ?? throw new NullReferenceException();

        content.Count.Should().Be(limit);
    }

    [Fact]
    public async Task IndexScansForAdmin_ReturnsOkAndCorrectData()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 300; _++) items.Add(PropertyItemFaker.Fake());

        var scans = new List<PropertyScan>();
        for (var _ = 0; _ < 300; _++) scans.Add(PropertyScanFaker.Fake(items));

        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake(scans: scans, items: items);
        var organization = OrganizationFaker.Fake(admin: admin, property: property);

        using var context = _app.GetDatabaseContext();

        context.Add(admin);
        context.Add(organization);
        context.Add(property);
        context.AddRange(items);
        context.AddRange(scans);

        context.SaveChanges();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
           new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act & Assert.
        var limit = 100;
        var offset = 100;
        var response = await _client.GetAsync($"/api/properties/scans?limit={limit}&offset={offset}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert that the response content is correct.
        var content = await response.Content.ReadFromJsonAsync<IndexForAdminPropertyScanQueryResponse>() ?? throw new NullReferenceException();

        content.Data.Count.Should().Be(limit);
        content.Limit.Should().Be(limit);
        content.Offset.Should().Be(offset);
        content.TotalCount.Should().Be(scans.Count);

        var scan = content.Data.First();

        scans.Should().Contain(s => s.Id == scan.Id);
        scan.ScannedItemsSummary.Should().NotBeNull();
        scan.ScannedItemsSummary!.TotalScannedItems.Should().Be(items.Count);
    }
}
