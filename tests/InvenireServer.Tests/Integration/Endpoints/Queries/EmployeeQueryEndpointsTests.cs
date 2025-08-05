using System.Net.Http.Json;
using System.Security.Claims;
using InvenireServer.Presentation;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Tests.Integration.Server;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Tests.Integration.Fakers.Users;
using InvenireServer.Tests.Integration.Fakers.Common;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Tests.Integration.Fakers.Properties;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Tests.Integration.Fakers.Properties.Items;

namespace InvenireServer.Tests.Integration.Endpoints.Queries;

public class EmployeeQueryEndpointsTests
{
    private readonly ServerFactory<Program> _app;
    private readonly HttpClient _client;
    private readonly JwtManager _jwt;

    public EmployeeQueryEndpointsTests()
    {
        _app = new ServerFactory<Program>();
        _jwt = JwtManagerFaker.Initiate();
        _client = _app.CreateDefaultClient();
    }

    [Fact]
    public async Task GetByJwt_ReturnsOkAndCorrectData()
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
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.FalseString)
        ]))}");

        // Act & Assert.
        var response = await _client.GetAsync("/api/employees/profile");
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
    public async Task GetInvitationsByEmployee_ReturnsOkAndCorrectData()
    {
        var employee = EmployeeFaker.Fake();
        var invitations = new List<OrganizationInvitation>();

        using var context = _app.GetDatabaseContext();
        for (int i = 0; i < 20; i++)
        {
            var admin = AdminFaker.Fake();
            var invitation = OrganizationInvitationFaker.Fake(employee: employee);
            var organization = OrganizationFaker.Fake(admin: admin, invitations: [invitation]);

            context.Add(admin);
            context.Add(invitation);
            context.Add(organization);
            invitations.Add(invitation);
        }
        context.Add(employee);
        context.SaveChanges();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act & Assert.
        var limit = 20;
        var offset = 0;
        var response = await _client.GetAsync($"/api/employees/invitations?limit={limit}&offset={offset}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert that the response content is correct.
        var content = await response.Content.ReadFromJsonAsync<List<OrganizationInvitationDto>>() ?? throw new NullReferenceException();

        content.Count.Should().Be(limit);
        content.Should().AllSatisfy(i =>
        {
            i.Employee.Should().NotBeNull();
            i.Employee!.Id.Should().Be(employee.Id);
        });
    }
}
