using System.Net.Http.Json;
using System.Security.Claims;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation;
using InvenireServer.Tests.Integration.Extensions.Organizations;
using InvenireServer.Tests.Integration.Extensions.Users;
using InvenireServer.Tests.Integration.Fakers.Common;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Properties;
using InvenireServer.Tests.Integration.Fakers.Properties.Items;
using InvenireServer.Tests.Integration.Fakers.Users;
using InvenireServer.Tests.Integration.Server;

namespace InvenireServer.Tests.Integration.Endpoints.Queries;

public class OrganizationQueryEndpointsTests
{
    private readonly ServerFactory<Program> _app;
    private readonly HttpClient _client;
    private readonly JwtManager _jwt;

    public OrganizationQueryEndpointsTests()
    {
        _app = new ServerFactory<Program>();
        _jwt = JwtManagerFaker.Initiate();
        _client = _app.CreateDefaultClient();
    }

    [Fact]
    public async Task GetByAdmin_ReturnsOkAndCorrectData()
    {
        // Prepare.
        var employees = new List<Employee>();
        for (int _ = 0; _ < 20; _++) employees.Add(EmployeeFaker.Fake());

        var members = employees.GetRange(0, employees.Count / 2);
        var invitees = employees.GetRange(employees.Count / 2, employees.Count / 2);

        var invitations = new List<OrganizationInvitation>();
        foreach (var invitee in invitees) invitations.Add(OrganizationInvitationFaker.Fake(employee: invitee));

        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, property: property, employees: members, invitations: invitations);

        using var context = _app.GetDatabaseContext();
        context.Add(admin);
        context.AddRange(employees);
        context.Add(organization);
        context.AddRange(invitations);
        context.Add(property);
        context.SaveChanges();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act
        var response = await _client.GetAsync("/api/organizations");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var content = await response.Content.ReadFromJsonAsync<OrganizationDto>() ?? throw new NullReferenceException();

        content.Id.Should().Be(organization.Id);
        content.Name.Should().Be(organization.Name);
        content.CreatedAt.Should().Be(organization.CreatedAt);
        content.LastUpdatedAt.Should().Be(organization.LastUpdatedAt);
        content.Admin.Should().NotBeNull();
        content.Admin!.Id.Should().Be(admin.Id);
        content.Property.Should().NotBeNull();
        content.Property!.Id.Should().Be(property.Id);
        content.Employees.Count.Should().Be(members.Count);
        content.Invitations.Count.Should().Be(invitations.Count);
    }

    [Fact]
    public async Task GetEmployeeById_ReturnsOkAndCorrectData()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (int i = 0; i < 100; i++) items.Add(PropertyItemFaker.Fake());

        var suggestions = new List<PropertySuggestion>();
        for (int i = 0; i < 10; i++) suggestions.Add(PropertySuggestionFaker.Fake());

        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake(items: items, suggestions: suggestions);
        var organization = OrganizationFaker.Fake(admin: admin, employees: [employee]);

        using var context = _app.GetDatabaseContext();
        context.Add(admin);
        context.Add(employee);
        context.Add(organization);
        context.AddRange(items);
        context.AddRange(suggestions);
        context.SaveChanges();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act & Assert.
        var response = await _client.GetAsync($"/api/organizations/employees/{employee.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert that the response content is correct.
        var content = await response.Content.ReadFromJsonAsync<EmployeeDto>() ?? throw new NullReferenceException();

        content.Id.Should().Be(employee.Id);
        content.OrganizationId.Should().Be(employee.OrganizationId);
        content.Name.Should().Be(employee.Name);
        content.EmailAddress.Should().Be(employee.EmailAddress);
        content.CreatedAt.Should().Be(employee.CreatedAt);
        content.LastUpdatedAt.Should().Be(employee.LastUpdatedAt);
        content.AssignedItems.Count.Should().Be(items.Count);
        content.Suggestions.Count.Should().Be(suggestions.Count);
    }

    [Fact]
    public async Task GetInvitationById_ReturnsOkAndCorrectData()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake(employee: employee);
        var organization = OrganizationFaker.Fake(admin: admin, invitations: [invitation]);

        using var context = _app.GetDatabaseContext();
        context.Add(admin);
        context.Add(employee);
        context.Add(organization);
        context.Add(invitation);
        context.SaveChanges();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act & Assert.
        var response = await _client.GetAsync($"/api/organizations/invitations/{invitation.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert that the response content is correct.
        var content = await response.Content.ReadFromJsonAsync<OrganizationInvitationDto>() ?? throw new NullReferenceException();

        content.Id.Should().Be(invitation.Id);
        content.OrganizationId.Should().Be(invitation.OrganizationId);
        content.Description.Should().Be(invitation.Description);
        content.CreatedAt.Should().Be(invitation.CreatedAt);
        content.LastUpdatedAt.Should().Be(invitation.LastUpdatedAt);
        content.Employee.Should().NotBeNull();
        content.Employee!.Id.Should().Be(employee.Id);
    }
}
