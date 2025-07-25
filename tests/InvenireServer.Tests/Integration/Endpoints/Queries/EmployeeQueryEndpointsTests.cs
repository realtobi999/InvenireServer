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
}
