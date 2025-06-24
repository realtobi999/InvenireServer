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
    private readonly HttpClient _client;
    private readonly IJwtManager _jwt;
    private readonly ServerFactory<Program> _app;

    public EmployeeEndpointsTests()
    {
        _app = new ServerFactory<Program>();
        _jwt = new JwtManagerFaker().Initiate();
        _client = _app.CreateDefaultClient();
    }

    [Fact]
    public async Task Register_Returns201AndEmployeeIsCreated()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeCommand());
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Assert that the token has all the necessary claims.
        var jwt = JwtBuilder.Parse(await response.Content.ReadAsStringAsync());
        jwt.Payload.Should().Contain(c => c.Type == "role" && c.Value == Jwt.Roles.EMPLOYEE);
        jwt.Payload.Should().Contain(c => c.Type == "admin_id" && c.Value == employee.Id.ToString());
        jwt.Payload.Should().Contain(c => c.Type == "is_verified" && c.Value == bool.FalseString);

        await using var context = _app.GetDatabaseContext();
        var createdEmployee = await context.Employees.FirstOrDefaultAsync(e => e.Id == employee.Id);

        // Assert that the employee is created in the database.
        createdEmployee.Should().NotBeNull();
        // Assert that the password is hashed.
        createdEmployee!.Password.Should().NotBe(employee.Password);
    }


    [Fact]
    public async Task SendVerification_Returns204AndEmailIsSentWithTheACorrectLink()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        var jwt = _jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.FalseString)
        ]);
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(jwt)}");

        (await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeCommand())).StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/auth/employee/email-verification/send", new object());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var email = (EmailSenderFaker)_app.Services.GetRequiredService<IEmailSender>();
        email.CapturedMessages.Count.Should().Be(1);
        var message = email.CapturedMessages[0];

        // Assert that the email is properly constructed and contains a verification link.
        message.To.Should().ContainSingle(t => t.Address == employee.EmailAddress);
        message.Subject.Should().Contain("verify your email");
        message.Body.Should().NotBeNull();
        message.Body.Should().MatchRegex(@"https?:\/\/[^\/]+\/verify-email\?token=([\w\-_.]+)");

        var match = Regex.Match(message.Body, @"https?:\/\/[^\/]+\/verify-email\?token=([\w\-_.]+)");
        var token = JwtBuilder.Parse(match.Groups[1].Value);

        // Assert that the token in the verification link contains all important claims.
        token.Payload.Should().Contain(c => c.Type == "role" && c.Value == nameof(Employee).ToUpper());
        token.Payload.Should().Contain(c => c.Type == "employee_id" && c.Value == employee.Id.ToString());
        token.Payload.Should().Contain(c => c.Type == "is_verified" && c.Value == bool.FalseString);
        token.Payload.Should().Contain(c => c.Type == "purpose" && c.Value == "email_verification");
    }

    [Fact]
    public async Task SendVerification_Returns429WhenTooManyRequestsHasBeenSent()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        var jwt = _jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.FalseString)
        ]);
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(jwt)}");

        (await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeCommand())).StatusCode.Should().Be(HttpStatusCode.Created);

        // Trigger the endpoints 3 times to enable the rare limiter.
        for (var i = 0; i < 3; i++) (await _client.PostAsJsonAsync("/api/auth/employee/email-verification/send", new object())).StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/auth/employee/email-verification/send", new object());
        response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }

    [Fact]
    public async Task ConfirmVerification_Returns204AndEmailIsVerified()
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

        var jwt = _jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.FalseString),
            new Claim("purpose", "email_verification"),
        ]);
        _client.DefaultRequestHeaders.Remove("Authorization");
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(jwt)}");

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/auth/employee/email-verification/confirm", new object());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await using var context = _app.GetDatabaseContext();
        var updatedEmployee = await context.Employees.FirstOrDefaultAsync(e => e.Id == employee.Id);

        // Assert that the employee has a verified email.
        updatedEmployee!.IsVerified.Should().Be(true);
    }

    [Fact]
    public async Task Login_Returns200AndJwtIsReturned()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        (await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.FalseString),
            new Claim("purpose", "email_verification")
        ]))}");
        (await _client.PostAsJsonAsync("/api/auth/employee/email-verification/confirm", new object())).StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act & Assert.
        var dto = new LoginEmployeeCommand
        {
            EmailAddress = employee.EmailAddress,
            Password = employee.Password
        };

        var response = await _client.PostAsJsonAsync("/api/auth/employee/login", dto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert that the JWT has all the necessary claims.
        var jwt = JwtBuilder.Parse(await response.Content.ReadAsStringAsync());
        jwt.Payload.Should().Contain(c => c.Type == "role");
        jwt.Payload.Should().Contain(c => c.Type == "employee_id" && c.Value == employee.Id.ToString());
        jwt.Payload.Should().Contain(c => c.Type == "is_verified" && c.Value == bool.TrueString);
    }

    [Fact]
    public async Task Login_Returns401WhenBadCredentials()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        (await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.FalseString),
            new Claim("purpose", "email_verification")
        ]))}");
        (await _client.PostAsJsonAsync("/api/auth/employee/email-verification/confirm", new object())).StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act & Assert.
        var dto = new LoginEmployeeCommand
        {
            EmailAddress = employee.EmailAddress,
            Password = new Faker().Internet.SecurePassword() // Generate a random password.
        };

        var response = await _client.PostAsJsonAsync("/api/auth/employee/login", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_Returns429WhenTooManyRequestsHasBeenSent()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        (await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeCommand())).StatusCode.Should().Be(HttpStatusCode.Created);
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {_jwt.Writer.Write(_jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.FalseString),
            new Claim("purpose", "email_verification")
        ]))}");
        (await _client.PostAsJsonAsync("/api/auth/employee/email-verification/confirm", new object())).StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Trigger the endpoints 5 times to enable the rare limiter.
        for (var i = 0; i < 5; i++)
            (await _client.PostAsJsonAsync("/api/auth/employee/login", new LoginEmployeeCommand
            {
                EmailAddress = employee.EmailAddress,
                Password = new Faker().Internet.SecurePassword() // Generate a random password.
            })).StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // Act & Assert.
        var dto = new LoginEmployeeCommand
        {
            EmailAddress = employee.EmailAddress,
            Password = employee.Password
        };

        var response = await _client.PostAsJsonAsync("/api/auth/employee/login", dto);
        response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }
}