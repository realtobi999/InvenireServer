using System.Net.Http.Json;
using System.Security.Claims;
using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation;
using InvenireServer.Tests.Integration.Extensions.Organizations;
using InvenireServer.Tests.Integration.Extensions.Users;
using InvenireServer.Tests.Integration.Fakers.Common;
using InvenireServer.Tests.Integration.Fakers.Organizations;
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
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        (await _client.PostAsJsonAsync("/api/admins/register", admin.ToRegisterAdminCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        admin.SetAsVerified(_app.GetDatabaseContext());
        (await _client.PostAsJsonAsync("/api/organizations", organization.ToCreateOrganizationCommand())).StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var response = await _client.GetAsync("/api/organizations");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert that the response content is correct.
        var content = await response.Content.ReadFromJsonAsync<OrganizationDto>() ?? throw new NullReferenceException();

        content.Id.Should().Be(organization.Id);
        content.Name.Should().Be(organization.Name);
        content.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        content.LastUpdatedAt.Should().BeNull();
        content.Admin.Should().NotBeNull();
        content.Admin!.Id.Should().Be(admin.Id);
        content.Admin!.OrganizationId.Should().Be(organization.Id);
        content.Admin!.Name.Should().Be(admin.Name);
        content.Admin!.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        content.Admin!.LastUpdatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        content.Property.Should().BeNull();
        content.Employees.Should().BeEmpty();
        content.Invitations.Should().BeEmpty();
    }
}
