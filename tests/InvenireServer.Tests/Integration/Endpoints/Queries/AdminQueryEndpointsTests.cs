using System.Net.Http.Json;
using System.Security.Claims;
using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation;
using InvenireServer.Tests.Integration.Fakers.Common;
using InvenireServer.Tests.Integration.Fakers.Users;
using InvenireServer.Tests.Integration.Server;

namespace InvenireServer.Tests.Integration.Endpoints.Queries;

public class AdminQueryEndpointsTests
{
    private readonly ServerFactory<Program> _app;
    private readonly HttpClient _client;
    private readonly JwtManager _jwt;

    public AdminQueryEndpointsTests()
    {
        _app = new ServerFactory<Program>();
        _jwt = JwtManagerFaker.Initiate();
        _client = _app.CreateDefaultClient();
    }

    [Fact]
    public async Task GetByJwt_ReturnsOkAndCorrectData()
    {
        // Prepare.
        var admin = AdminFaker.Fake();

        using var context = _app.GetDatabaseContext();
        context.Add(admin);
        context.SaveChanges();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("is_verified", bool.FalseString)
        ]))}");

        // Act & Assert.
        var response = await _client.GetAsync("/api/admins/profile");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert that the response content is correct.
        var content = await response.Content.ReadFromJsonAsync<AdminDto>() ?? throw new NullReferenceException();

        content.Id.Should().Be(admin.Id);
        content.OrganizationId.Should().BeNull();
        content.Name.Should().Be(admin.Name);
        content.EmailAddress.Should().Be(admin.EmailAddress);
        content.CreatedAt.Should().Be(admin.CreatedAt);
        content.LastUpdatedAt.Should().Be(admin.LastUpdatedAt);
    }
}
