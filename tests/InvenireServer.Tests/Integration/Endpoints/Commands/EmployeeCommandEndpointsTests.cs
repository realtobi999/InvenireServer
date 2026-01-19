using System.Net.Http.Json;
using System.Security.Claims;
using InvenireServer.Application.Core.Employees.Commands.Login;
using InvenireServer.Application.Core.Employees.Commands.Update;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation;
using InvenireServer.Tests.Extensions.Users;
using InvenireServer.Tests.Fakers.Common;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Integration.Server;

namespace InvenireServer.Tests.Integration.Endpoints.Commands;

/// <summary>
/// Integration tests for employee command endpoints.
/// </summary>
public class EmployeeCommandEndpointsTests
{
    private readonly ServerFactory<Program> _app;
    private readonly HttpClient _client;
    private readonly JwtManager _jwt;

    public EmployeeCommandEndpointsTests()
    {
        _app = new ServerFactory<Program>();
        _jwt = JwtManagerFaker.Initiate();
        _client = _app.CreateDefaultClient();
    }

    /// <summary>
    /// Verifies that the employee registration endpoint returns Created.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Register_ReturnsCreated()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/employees/register", employee.ToRegisterEmployeeCommand());
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }


    /// <summary>
    /// Verifies that the verification email endpoint returns NoContent for an unverified employee.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task SendVerification_ReturnsNoContent()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.FalseString)
        ]))}");

        (await _client.PostAsJsonAsync("/api/employees/register", employee.ToRegisterEmployeeCommand())).StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/employees/email-verification/send", new object());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Verifies that the verification confirmation endpoint returns NoContent with a verification token.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task ConfirmVerification_ReturnsNoContent()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.FalseString)
        ]))}");

        (await _client.PostAsJsonAsync("/api/employees/register", employee.ToRegisterEmployeeCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/employees/email-verification/send", new object())).StatusCode.Should().Be(HttpStatusCode.NoContent);

        _client.DefaultRequestHeaders.Remove("Authorization");
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.FalseString),
            new Claim("purpose", "email_verification")
        ]))}");

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/employees/email-verification/confirm", new object());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Verifies that the employee login endpoint returns NoContent for valid credentials.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Login_ReturnsNoContent()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();

        (await _client.PostAsJsonAsync("/api/employees/register", employee.ToRegisterEmployeeCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        employee.SetAsVerified(_app.GetDatabaseContext());

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/employees/login", new LoginEmployeeCommand
        {
            EmailAddress = employee.EmailAddress,
            Password = employee.Password
        });
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Verifies that the employee update endpoint returns NoContent for an authorized employee.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Update_ReturnsNoContent()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();

        (await _client.PostAsJsonAsync("/api/employees/register", employee.ToRegisterEmployeeCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        employee.SetAsVerified(_app.GetDatabaseContext());

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act & Assert.
        var response = await _client.PutAsJsonAsync("/api/employees", new UpdateEmployeeCommand
        {
            FirstName = new Faker().Name.FirstName(),
            LastName = new Faker().Name.LastName(),
        });
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Verifies that the employee delete endpoint returns NoContent for an authorized employee.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();

        (await _client.PostAsJsonAsync("/api/employees/register", employee.ToRegisterEmployeeCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        employee.SetAsVerified(_app.GetDatabaseContext());

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]))}");

        // Act & Assert.
        var response = await _client.DeleteAsync("/api/employees");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
