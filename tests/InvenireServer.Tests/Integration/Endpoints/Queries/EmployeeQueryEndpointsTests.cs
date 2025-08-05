using System.Net.Http.Json;
using System.Security.Claims;
using InvenireServer.Presentation;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Tests.Integration.Server;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Tests.Integration.Fakers.Users;
using InvenireServer.Tests.Integration.Fakers.Common;
using InvenireServer.Tests.Integration.Extensions.Users;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Extensions.Organizations;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Application.Dtos.Organizations;

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
        var employee = EmployeeFaker.Fake();

        (await _client.PostAsJsonAsync("/api/employees/register", employee.ToRegisterEmployeeCommand())).StatusCode.Should().Be(HttpStatusCode.Created);

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
        content.OrganizationId.Should().BeNull();
        content.Name.Should().Be(employee.Name);
        content.EmailAddress.Should().Be(employee.EmailAddress);
        content.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        content.LastUpdatedAt.Should().BeNull();
        content.AssignedItems.Should().BeEmpty();
        content.Suggestions.Should().BeEmpty();
    }

    [Fact]
    public async Task GetInvitationsByEmployee_ReturnsOkAndCorrectData()
    {
        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake();

        var invitations = new List<OrganizationInvitation>();
        for (int i = 0; i < 20; i++) invitations.Add(OrganizationInvitationFaker.Fake(employee: employee));

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

        foreach (var invitation in invitations)
            (await _client.PostAsJsonAsync("/api/organizations/invitations", invitation.ToCreateOrganizationInvitationCommand())).StatusCode.Should().Be(HttpStatusCode.Created);

        _client.DefaultRequestHeaders.Remove("Authorization");
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act & Assert.
        var limit = invitations.Count;
        var offset = 0;
        var response = await _client.GetAsync($"/api/employees/invitations?limit={limit}&offset={offset}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert that the response content is correct.
        var content = await response.Content.ReadFromJsonAsync<List<OrganizationInvitationDto>>() ?? throw new NullReferenceException();

        foreach (var invitation in content)
        {
            invitations.Select(i => i.Id).Should().Contain(invitation.Id);
            invitation.OrganizationId.Should().Be(organization.Id);
            invitations.Select(i => i.Description).Should().Contain(invitation.Description);
            invitation.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
            invitation.LastUpdatedAt.Should().BeNull();
            invitation.Employee.Should().NotBeNull();
            invitation.Employee!.Id.Should().Be(employee.Id);
            invitation.Employee!.OrganizationId.Should().BeNull();
            invitation.Employee!.Name.Should().Be(employee.Name);
            invitation.Employee!.EmailAddress.Should().Be(employee.EmailAddress);
            invitation.Employee!.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
            invitation.Employee!.LastUpdatedAt.Should().BeNull();
            invitation.Employee!.AssignedItems.Should().BeEmpty();
            invitation.Employee!.Suggestions.Should().BeEmpty();
        }
    }
}
