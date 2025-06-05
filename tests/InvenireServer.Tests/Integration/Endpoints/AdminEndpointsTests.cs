using System.Net.Http.Json;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Presentation;
using InvenireServer.Tests.Integration.Extensions;
using InvenireServer.Tests.Integration.Fakers;
using InvenireServer.Tests.Integration.Server;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Tests.Integration.Endpoints;

public class AdminEndpointsTests
{
    private readonly HttpClient _client;
    private readonly IJwtManager _jwt;
    private readonly ServerFactory<Program> _app;

    public AdminEndpointsTests()
    {
        _app = new ServerFactory<Program>();
        _jwt = new JwtManagerFaker().Initiate();
        _client = _app.CreateDefaultClient();
    }

    [Fact]
    public async Task RegisterAdmin_Returns201AndAdminIsCreated()
    {
        // Prepare.
        var admin = new AdminFaker().Generate();

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/auth/admin/register", admin.ToRegisterAdminDto());
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await using var context = _app.GetDatabaseContext();
        var createdAdmin = await context.Admins.FirstOrDefaultAsync(e => e.Id == admin.Id);

        // Assert that the admin is created in the database.
        createdAdmin.Should().NotBeNull();
        // Assert that the password is hashed.
        createdAdmin!.Password.Should().NotBe(admin.Password);
    }
}
