using System.Net.Http.Json;
using System.Security.Claims;
using InvenireServer.Application.Core.Employees.Commands.Login;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation;
using InvenireServer.Tests.Integration.Extensions.Users;
using InvenireServer.Tests.Integration.Fakers.Common;
using InvenireServer.Tests.Integration.Fakers.Users;
using InvenireServer.Tests.Integration.Server;

namespace InvenireServer.Tests.Integration.Endpoints;

public class EmployeeEndpointsTests
{
    private readonly ServerFactory<Program> _app;
    private readonly HttpClient _client;
    private readonly JwtManager _jwt;

    public EmployeeEndpointsTests()
    {
        _app = new ServerFactory<Program>();
        _jwt = new JwtManagerFaker().Initiate();
        _client = _app.CreateDefaultClient();
    }

    [Fact]
    public async Task Register_ReturnsCreated()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeCommand());
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }


    [Fact]
    public async Task SendVerification_ReturnsNoContent()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.FalseString)
        ]))}");

        (await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeCommand())).StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/auth/employee/email-verification/send", new object());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ConfirmVerification_ReturnsNoContent()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.FalseString)
        ]))}");

        (await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/auth/employee/email-verification/send", new object())).StatusCode.Should().Be(HttpStatusCode.NoContent);

        _client.DefaultRequestHeaders.Remove("Authorization");
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.FalseString),
            new Claim("purpose", "email_verification")
        ]))}");

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/auth/employee/email-verification/confirm", new object());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Login_ReturnsOk()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.FalseString),
            new Claim("purpose", "email_verification")
        ]))}");

        (await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/auth/employee/email-verification/confirm", new object())).StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act & Assert.

        var response = await _client.PostAsJsonAsync("/api/auth/employee/login", new LoginEmployeeCommand
        {
            EmailAddress = employee.EmailAddress,
            Password = employee.Password
        });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}