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
using InvenireServer.Tests.Fakers.Common;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Properties;
using InvenireServer.Tests.Fakers.Properties.Items;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Integration.Server;

namespace InvenireServer.Tests.Integration.Endpoints.Queries;

/// <summary>
/// Integration tests for organization query endpoints.
/// </summary>
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

    /// <summary>
    /// Verifies that the organizations endpoint returns OK and the expected organization data for an admin.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
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
        content.Employees.Should().NotBeNullOrEmpty();
        content.Employees!.Count.Should().Be(members.Count);
        content.Invitations.Should().NotBeNullOrEmpty();
        content.Invitations!.Count.Should().Be(invitations.Count);
    }

    /// <summary>
    /// Verifies that the organizations endpoint returns OK and the expected organization data.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task GetById_ReturnsOkAndCorrectData()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin);

        using var context = _app.GetDatabaseContext();
        context.Add(admin);
        context.Add(organization);
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
    }

    /// <summary>
    /// Verifies that the employee by id endpoint returns OK and the expected data.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task GetEmployeeById_ReturnsOkAndCorrectData()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, employees: [employee]);

        using var context = _app.GetDatabaseContext();
        context.Add(admin);
        context.Add(employee);
        context.Add(organization);
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
        content.FirstName.Should().Be(employee.FirstName);
        content.LastName.Should().Be(employee.LastName);
        content.EmailAddress.Should().Be(employee.EmailAddress);
        content.CreatedAt.Should().Be(employee.CreatedAt);
        content.LastUpdatedAt.Should().Be(employee.LastUpdatedAt);
    }

    /// <summary>
    /// Verifies that the employee by email address endpoint returns OK and the expected data.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task GetEmployeeByEmailAddress_ReturnsOkAndCorrectData()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, employees: [employee]);

        using var context = _app.GetDatabaseContext();
        context.Add(admin);
        context.Add(employee);
        context.Add(organization);
        context.SaveChanges();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act & Assert.
        var response = await _client.GetAsync($"/api/organizations/employees/{employee.EmailAddress}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert that the response content is correct.
        var content = await response.Content.ReadFromJsonAsync<EmployeeDto>() ?? throw new NullReferenceException();

        content.Id.Should().Be(employee.Id);
        content.OrganizationId.Should().Be(employee.OrganizationId);
        content.FirstName.Should().Be(employee.FirstName);
        content.LastName.Should().Be(employee.LastName);
        content.EmailAddress.Should().Be(employee.EmailAddress);
        content.CreatedAt.Should().Be(employee.CreatedAt);
        content.LastUpdatedAt.Should().Be(employee.LastUpdatedAt);
    }

    /// <summary>
    /// Verifies that the invitation by id endpoint returns OK and the expected data.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
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
