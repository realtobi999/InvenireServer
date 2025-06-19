using System.Net.Http.Json;
using System.Security.Claims;
using InvenireServer.Application.Interfaces.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Presentation;
using InvenireServer.Tests.Integration.Extensions;
using InvenireServer.Tests.Integration.Fakers;
using InvenireServer.Tests.Integration.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InvenireServer.Tests.Integration.Endpoints;

public class OrganizationEndpointsTests
{
    private readonly ServerFactory<Program> _app;
    private readonly HttpClient _client;
    private readonly IJwtManager _jwt;

    public OrganizationEndpointsTests()
    {
        _app = new ServerFactory<Program>();
        _jwt = new JwtManagerFaker().Initiate();
        _client = _app.CreateDefaultClient();
    }

    [Fact]
    public async Task CreateOrganization_Returns201AndOrganizationIsCreated()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var admin = new AdminFaker(organization).Generate();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        (await _client.PostAsJsonAsync("/api/auth/admin/register", admin.ToRegisterAdminDto())).StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/organizations", organization.ToCreateOrganizationDto());
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await using var context = _app.GetDatabaseContext();
        var updatedAdmin = await context.Admins.FirstOrDefaultAsync(a => a.Id == admin.Id);
        var createdOrganization = await context.Organizations.Include(o => o.Admin).FirstOrDefaultAsync(o => o.Id == organization.Id);

        // Assert that the organization is created.
        createdOrganization.Should().NotBeNull();
        // Assert that the admin is assigned.
        createdOrganization!.Admin.Should().NotBeNull();
        createdOrganization.Admin!.Id.Should().Be(admin.Id);
        // Assert that the admin has a assigned organization.
        updatedAdmin.Should().NotBeNull();
        updatedAdmin!.OrganizationId.Should().Be(organization.Id);

        var email = (EmailSenderFaker)_app.Services.GetRequiredService<IEmailSender>();
        email.CapturedMessages.Count.Should().Be(1);

        var message = email.CapturedMessages[0];

        // Assert that the email is properly constructed;
        message.Should().NotBeNull();
        message.To.Should().ContainSingle(t => t.Address == admin.EmailAddress);
        message.Subject.Should().Contain("organization creation");
        message.Body.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateOrganizationInvitation_Returns201AndInvitationIsCreated()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var admin = new AdminFaker(organization).Generate();
        var employee = new EmployeeFaker(organization).Generate();
        var invitation = new OrganizationInvitationFaker(organization, employee).Generate();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        (await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeDto())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/auth/admin/register", admin.ToRegisterAdminDto())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/organizations", organization.ToCreateOrganizationDto())).StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var response = await _client.PostAsJsonAsync($"/api/organizations/{organization.Id}/invitations", invitation.ToCreateOrganizationInvitationDto());
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await using var context = _app.GetDatabaseContext();
        var createdInvitation = await context.Invitations.Include(i => i.Employee).FirstOrDefaultAsync(i => i.Id == invitation.Id);
        var updatedOrganization = await context.Organizations.Include(o => o.Invitations).FirstOrDefaultAsync(o => o.Id == organization.Id);

        // Assert that the invitation is created.
        createdInvitation.Should().NotBeNull();
        // Assert that the organization and employee is assigned.
        createdInvitation!.OrganizationId.Should().Be(organization.Id);
        createdInvitation!.Employee.Should().NotBeNull();
        createdInvitation!.Employee!.Id.Should().Be(employee.Id);
        // Assert that the organization has a assigned invitation.
        updatedOrganization!.Invitations!.Should().ContainSingle(i => i.Id == invitation.Id);
    }

    [Fact]
    public async Task AcceptOrganizationInvitation_Returns204AndEmployeeIsPartOfOrganization()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var admin = new AdminFaker(organization).Generate();
        var employee = new EmployeeFaker(organization).Generate();
        var invitation = new OrganizationInvitationFaker(organization, employee).Generate();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        (await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeDto())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/auth/admin/register", admin.ToRegisterAdminDto())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/organizations", organization.ToCreateOrganizationDto())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync($"/api/organizations/{organization.Id}/invitations", invitation.ToCreateOrganizationInvitationDto())).StatusCode.Should().Be(HttpStatusCode.Created);

        _client.DefaultRequestHeaders.Remove("Authorization");
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act & Assert.
        var response = await _client.PostAsJsonAsync($"/api/organizations/{organization.Id}/invitations/{invitation.Id}/accept", new object());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await using var context = _app.GetDatabaseContext();
        var deletedInvitation = await context.Invitations.FirstOrDefaultAsync(i => i.Id == invitation.Id);
        var updatedOrganization = await context.Organizations.Include(o => o.Invitations).Include(o => o.Employees).FirstOrDefaultAsync(o => o.Id == organization.Id);
        var updatedEmployee = await context.Employees.FirstOrDefaultAsync(e => e.Id == employee.Id);

        // Assert that the employee is a part of the organization.
        updatedEmployee!.OrganizationId.Should().Be(organization.Id);
        updatedOrganization!.Employees.Should().ContainSingle(e => e.Id == employee.Id);
        // Assert that the invitation is deleted.
        deletedInvitation.Should().BeNull();
        updatedOrganization!.Invitations.Should().NotContain(i => i.Id == invitation.Id);
    }
}