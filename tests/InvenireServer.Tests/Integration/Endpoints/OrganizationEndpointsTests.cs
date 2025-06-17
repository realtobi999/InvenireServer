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
    private readonly HttpClient _client;
    private readonly IJwtManager _jwt;
    private readonly ServerFactory<Program> _app;

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

        var jwt = _jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString),
        ]);
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(jwt)}");

        (await _client.PostAsJsonAsync("/api/auth/admin/register", admin.ToRegisterAdminDto())).StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/organization", organization.ToCreateOrganizationDto());
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
}
