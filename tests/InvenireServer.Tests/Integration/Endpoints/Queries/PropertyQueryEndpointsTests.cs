using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using ClosedXML.Excel;
using InvenireServer.Application.Core.Properties.Items.Queries.IndexByAdmin;
using InvenireServer.Application.Core.Properties.Items.Queries.IndexByScan;
using InvenireServer.Application.Core.Properties.Scans.Queries.IndexByAdmin;
using InvenireServer.Application.Core.Properties.Suggestions.Commands;
using InvenireServer.Application.Core.Properties.Suggestions.Queries.IndexByAdmin;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation;
using InvenireServer.Tests.Fakers.Common;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Properties;
using InvenireServer.Tests.Fakers.Properties.Items;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Integration.Server;

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
        for (var _ = 0; _ < 100; _++) items.Add(PropertyItemFaker.Fake());

        var suggestions = new List<PropertySuggestion>();
        for (var _ = 0; _ < 10; _++) suggestions.Add(PropertySuggestionFaker.Fake());

        var scans = new List<PropertyScan>();
        for (var _ = 0; _ < 10; _++) scans.Add(PropertyScanFaker.Fake());

        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake(items: items, suggestions: suggestions);
        var property = PropertyFaker.Fake(items: items, suggestions: suggestions, scans: scans);
        var organization = OrganizationFaker.Fake(admin: admin, property: property);

        using var context = _app.GetDatabaseContext();
        context.Add(admin);
        context.Add(employee);
        context.Add(organization);
        context.Add(property);
        context.AddRange(items);
        context.AddRange(suggestions);
        context.AddRange(scans);
        context.SaveChanges();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act & Assert.
        var response = await _client.GetAsync("/api/properties");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert that the response content is correct.
        var content = await response.Content.ReadFromJsonAsync<PropertyDto>() ?? throw new NullReferenceException();

        content.Id.Should().Be(property.Id);
        content.OrganizationId.Should().Be(property.OrganizationId);
        content.CreatedAt.Should().Be(property.CreatedAt);
        content.LastUpdatedAt.Should().Be(property.LastUpdatedAt);
        content.ItemsSummary.Should().NotBeNull();
        content.ItemsSummary!.TotalItems.Should().Be(items.Count);
        content.ItemsSummary!.TotalValue.Should().Be(items.Sum(i => i.Price));
        content.ScansSummary.Should().NotBeNull();
        content.ScansSummary!.TotalScans.Should().Be(scans.Count);
        content.SuggestionsSummary.Should().NotBeNull();
        content.SuggestionsSummary!.TotalSuggestions.Should().Be(suggestions.Count);
    }

    [Fact]
    public async Task IndexItemsByAdmin_ReturnsOkAndCorrectData()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 100; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake(items: items);
        var organization = OrganizationFaker.Fake(admin: admin, property: property);

        using var context = _app.GetDatabaseContext();
        context.Add(admin);
        context.Add(organization);
        context.Add(property);
        context.AddRange(items);
        context.SaveChanges();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act & Assert.
        var limit = 100;
        var offset = 0;
        var response = await _client.GetAsync($"/api/properties/items?limit={limit}&offset={offset}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert that the response content is correct.
        var content = await response.Content.ReadFromJsonAsync<IndexByAdminPropertyItemQueryResponse>() ?? throw new NullReferenceException();

        content.Data.Should().NotBeEmpty();
        content.Limit.Should().Be(limit);
        content.Offset.Should().Be(offset);
        content.TotalCount.Should().Be(items.Count);
    }

    [Fact]
    public async Task ExportItemsToExcel_ReturnsOkAndCorrectData()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 100; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake(items: items);
        var property = PropertyFaker.Fake(items: items);
        var organization = OrganizationFaker.Fake(admin: admin, property: property, employees: [employee]);

        using var context = _app.GetDatabaseContext();
        context.Add(admin);
        context.Add(employee);
        context.Add(organization);
        context.Add(property);
        context.AddRange(items);
        context.SaveChanges();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act & Assert.
        var response = await _client.GetAsync("/api/properties/items/export/excel");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.ToString().Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        response.Content.Headers.ContentLength.Should().BeGreaterThan(0);

        var content = await response.Content.ReadAsStreamAsync();
        using var workbook = new XLWorkbook(content);
        var worksheet = workbook.Worksheets.First();

        var rows = worksheet.RowsUsed().Skip(1).ToList();
        rows.Count.Should().Be(items.Count);

        foreach (var row in rows.Take(5))
        {
            var item = items.FirstOrDefault(i => i.InventoryNumber == row.Cell(1).GetString());
            item.Should().NotBeNull();
            row.Cell(2).GetString().Should().Be(item!.RegistrationNumber);
            row.Cell(3).GetString().Should().Be(item.Name);
            row.Cell(4).GetString().Should().Be(item.Price.ToString());
            row.Cell(5).GetString().Should().Be(item.SerialNumber ?? "");
            row.Cell(6).GetString().Should().Be(item.DateOfPurchase.ToString("yyyy-MM-dd"));
            row.Cell(7).GetString().Should().Be(item.DateOfSale?.ToString("yyyy-MM-dd") ?? "");
            row.Cell(8).GetString().Should().Be(item.Location.Building);
            row.Cell(9).GetString().Should().Be(item.Location.Room);
            row.Cell(10).GetString().Should().Be(item.Location.AdditionalNote ?? "");
            row.Cell(11).GetString().Should().Be(employee.Id.ToString() ?? "");
            row.Cell(12).GetString().Should().Be($"{employee.FirstName} {employee.LastName}");
            row.Cell(13).GetString().Should().Be(employee.EmailAddress);
            row.Cell(14).GetString().Should().Be(item.Description ?? "");
            row.Cell(15).GetString().Should().Be(item.DocumentNumber ?? "");
        }
    }

    [Fact]
    public async Task ExportItemsToJson_ReturnsOkAndCorrectData()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 100; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake(items: items);
        var property = PropertyFaker.Fake(items: items);
        var organization = OrganizationFaker.Fake(admin: admin, property: property, employees: [employee]);

        using var context = _app.GetDatabaseContext();
        context.Add(admin);
        context.Add(employee);
        context.Add(organization);
        context.Add(property);
        context.AddRange(items);
        context.SaveChanges();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act & Assert.
        var response = await _client.GetAsync("/api/properties/items/export/json");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.ToString().Should().Be("application/json");
        response.Content.Headers.ContentLength.Should().BeGreaterThan(0);

        var content = await response.Content.ReadAsStreamAsync();
        JsonSerializer.Deserialize<List<PropertyItemDto>>(content)!.Count.Should().Be(items.Count);
    }

    [Fact]
    public async Task GetItemById_ReturnsOkAndCorrectData()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 100; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake(items: items);
        var organization = OrganizationFaker.Fake(admin: admin, property: property);

        using var context = _app.GetDatabaseContext();
        context.Add(admin);
        context.Add(organization);
        context.Add(property);
        context.AddRange(items);
        context.SaveChanges();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act & Assert.
        var item = items.First();
        var response = await _client.GetAsync($"/api/properties/items/{item.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert that the response content is correct.
        var content = await response.Content.ReadFromJsonAsync<PropertyItemDto>() ?? throw new NullReferenceException();

        content.Id.Should().Be(item.Id);
        content.PropertyId.Should().Be(item.PropertyId);
        content.EmployeeId.Should().Be(item.EmployeeId);
        content.InventoryNumber.Should().Be(item.InventoryNumber);
        content.RegistrationNumber.Should().Be(item.RegistrationNumber);
        content.Name.Should().Be(item.Name);
        content.Price.Should().Be(item.Price);
        content.SerialNumber.Should().Be(item.SerialNumber);
        content.DateOfPurchase.Should().Be(item.DateOfPurchase);
        content.DateOfSale.Should().Be(item.DateOfSale);
        content.Location.Should().BeEquivalentTo(item.Location);
        content.Description.Should().Be(item.Description);
        content.DocumentNumber.Should().Be(item.DocumentNumber);
        content.CreatedAt.Should().Be(item.CreatedAt);
        content.LastUpdatedAt.Should().Be(item.LastUpdatedAt);
    }


    [Fact]
    public async Task IndexScansByAdmin_ReturnsOkAndCorrectData()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 100; _++) items.Add(PropertyItemFaker.Fake());

        var scans = new List<PropertyScan>();
        for (var _ = 0; _ < 10; _++) scans.Add(PropertyScanFaker.Fake(items));

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
        var limit = 10;
        var offset = 0;
        var response = await _client.GetAsync($"/api/properties/scans?limit={limit}&offset={offset}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert that the response content is correct.
        var content = await response.Content.ReadFromJsonAsync<IndexByAdminPropertyScanQueryResponse>() ?? throw new NullReferenceException();

        content.Data.Should().AllSatisfy(s =>
        {
            s.ScannedItemsSummary.Should().NotBeNull();
            s.ScannedItemsSummary!.TotalItemsToScan.Should().Be(items.Count);
        });
        content.Limit.Should().Be(limit);
        content.Offset.Should().Be(offset);
        content.TotalCount.Should().Be(scans.Count);
    }

    [Fact]
    public async Task IndexItemsByScan_ReturnsOkAndCorrectData()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 100; _++) items.Add(PropertyItemFaker.Fake());

        var scan = PropertyScanFaker.Fake(items: items);

        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake(scans: [scan], items: items);
        var organization = OrganizationFaker.Fake(admin: admin, property: property);

        using var context = _app.GetDatabaseContext();
        context.Add(admin);
        context.Add(organization);
        context.Add(property);
        context.AddRange(items);
        context.Add(scan);
        context.SaveChanges();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
           new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act & Assert.
        var limit = 10;
        var offset = 0;
        var response = await _client.GetAsync($"/api/properties/scans/{scan.Id}/items?limit={limit}&offset={offset}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert that the response content is correct.
        var content = await response.Content.ReadFromJsonAsync<IndexByScanPropertyItemQueryResponse>() ?? throw new NullReferenceException();

        content.Data.Should().NotBeEmpty();
        content.Limit.Should().Be(limit);
        content.Offset.Should().Be(offset);
        content.TotalCount.Should().Be(items.Count);
    }

    [Fact]
    public async Task IndexSuggestionsByAdmin_ReturnsOkAndCorrectData()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 100; _++) items.Add(PropertyItemFaker.Fake());

        var suggestions = new List<PropertySuggestion>();
        for (var _ = 0; _ < 20; _++)
        {
            var suggestion = PropertySuggestionFaker.Fake();
            suggestion.PayloadString = JsonSerializer.Serialize(new PropertySuggestionPayload
            {
                DeleteCommands = [],
                CreateCommands = [],
                UpdateCommands = []
            });
            suggestions.Add(suggestion);
        }

        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake(suggestions: suggestions);
        var property = PropertyFaker.Fake(suggestions: suggestions, items: items);
        var organization = OrganizationFaker.Fake(admin: admin, property: property, employees: [employee]);

        using var context = _app.GetDatabaseContext();
        context.Add(admin);
        context.Add(employee);
        context.Add(organization);
        context.Add(property);
        context.AddRange(items);
        context.AddRange(suggestions);
        context.SaveChanges();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
           new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act & Assert.
        var limit = 10;
        var offset = 0;
        var response = await _client.GetAsync($"/api/properties/suggestions?limit={limit}&offset={offset}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert that the response content is correct.
        var content = await response.Content.ReadFromJsonAsync<IndexByAdminPropertySuggestionQueryResponse>() ?? throw new NullReferenceException();

        content.Data.Should().NotBeEmpty();
        content.Limit.Should().Be(limit);
        content.Offset.Should().Be(offset);
        content.TotalCount.Should().Be(suggestions.Count);
    }
}
