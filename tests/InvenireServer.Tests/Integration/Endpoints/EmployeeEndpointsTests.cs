using System.Net.Http.Json;
using InvenireServer.Presentation;
using InvenireServer.Tests.Integration.Extensions;
using InvenireServer.Tests.Integration.Fakers;
using InvenireServer.Tests.Integration.Server;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Tests.Integration.Endpoints;

public class EmployeeEndpointsTests
{
    [Fact]
    public async Task RegisterEmployee_Returns201AndEmployeeIsCreated()
    {
        // Prepare.
        var app = new ServerFactory<Program>();
        var client = app.CreateDefaultClient();
        var employee = new EmployeeFaker().Generate();

        // Act & Assert.
        var response = await client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeDto());
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        using var context = app.GetDatabaseContext();
        var createdEmployee = await context.Employees.FirstOrDefaultAsync(e => e.Id == employee.Id);

        // Assert that the employee is created in the database.
        createdEmployee.Should().NotBeNull();
        // Assert that the password is hashed.
        createdEmployee!.Password.Should().NotBe(employee.Password);
        // Assert that the login lock is set-up correctly.
        createdEmployee!.LoginAttempts.Should().Be(0);
        createdEmployee!.LoginLock.IsSet.Should().BeFalse();
        createdEmployee!.LoginLock.ExpirationDate.Should().BeNull();
    }
}
