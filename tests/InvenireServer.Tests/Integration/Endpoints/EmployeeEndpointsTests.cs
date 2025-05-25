using System.Net.Http.Json;
using InvenireServer.Presentation;
using Microsoft.EntityFrameworkCore;
using InvenireServer.Tests.Integration.Fakers;
using InvenireServer.Tests.Integration.Server;
using InvenireServer.Domain.Core.Dtos.Employees;
using InvenireServer.Domain.Core.Entities.Common;
using InvenireServer.Tests.Integration.Extensions;

namespace InvenireServer.Tests.Integration.Endpoints;

public class EmployeeEndpointsTests
{
    private readonly HttpClient _client;
    private readonly ServerFactory<Program> _app;

    public EmployeeEndpointsTests()
    {
        _app = new ServerFactory<Program>();
        _client = _app.CreateDefaultClient();
    }

    [Fact]
    public async Task RegisterEmployee_Returns201AndEmployeeIsCreated()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeDto());
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        using var context = _app.GetDatabaseContext();
        var createdEmployee = await context.Employees.FirstOrDefaultAsync(e => e.Id == employee.Id);

        // Assert that the employee is created in the database.
        createdEmployee.Should().NotBeNull();
        // Assert that the password is hashed.
        createdEmployee!.Password.Should().NotBe(employee.Password);
    }

    [Fact]
    public async Task LoginEmployee_Returns200AndJwtIsReturned()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        var create1 = await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeDto());
        create1.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var dto = new LoginEmployeeDto
        {
            EmailAddress = employee.EmailAddress,
            Password = employee.Password,
        };

        var response = await _client.PostAsJsonAsync("/api/auth/employee/login", dto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<LoginEmployeeResponseDto>();
        body.Should().NotBeNull();

        var jwt = Jwt.Parse(body!.Token);
        jwt.Payload.Should().Contain(c => c.Type == "role");
        jwt.Payload.Should().Contain(c => c.Type == "employee_id" && c.Value == employee.Id.ToString());
    }

    [Fact]
    public async Task LoginEmployee_Returns401WhenBadCredentials()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        var create1 = await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeDto());
        create1.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var dto = new LoginEmployeeDto
        {
            EmailAddress = employee.EmailAddress,
            Password = new Faker().Internet.SecurePassword(), // Generate a random password.
        };

        var response = await _client.PostAsJsonAsync("/api/auth/employee/login", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
