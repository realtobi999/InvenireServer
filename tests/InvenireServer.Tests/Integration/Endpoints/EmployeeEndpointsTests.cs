using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.RegularExpressions;
using InvenireServer.Application.Core.Employees.Commands.Login;
using InvenireServer.Application.Interfaces.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation;
using InvenireServer.Tests.Integration.Extensions;
using InvenireServer.Tests.Integration.Extensions.Users;
using InvenireServer.Tests.Integration.Fakers.Common;
using InvenireServer.Tests.Integration.Fakers.Users;
using InvenireServer.Tests.Integration.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InvenireServer.Tests.Integration.Endpoints;

public class EmployeeEndpointsTests
{
    private readonly JwtManager _jwt;
    private readonly HttpClient _client;
    private readonly ServerFactory<Program> _app;

    public EmployeeEndpointsTests()
    {
        _app = new ServerFactory<Program>();
        _jwt = new JwtManagerFaker().Initiate();
        _client = _app.CreateDefaultClient();
    }

    [Fact]
    public async Task Register_Returns201()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeCommand());
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }


    [Fact]
    public async Task SendVerification_Returns204()
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
    public async Task ConfirmVerification_Returns204()
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
    public async Task Login_Returns200()
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
        var dto = new LoginEmployeeCommand
        {
            EmailAddress = employee.EmailAddress,
            Password = employee.Password
        };

        var response = await _client.PostAsJsonAsync("/api/auth/employee/login", dto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}