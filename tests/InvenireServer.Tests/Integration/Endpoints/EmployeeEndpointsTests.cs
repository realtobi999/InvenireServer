using System.Net.Http.Json;
using InvenireServer.Presentation;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using InvenireServer.Domain.Core.Entities;
using InvenireServer.Tests.Integration.Fakers;
using InvenireServer.Tests.Integration.Server;
using Microsoft.Extensions.DependencyInjection;
using InvenireServer.Application.Core.Factories;
using InvenireServer.Domain.Core.Dtos.Employees;
using InvenireServer.Domain.Core.Entities.Common;
using InvenireServer.Tests.Integration.Extensions;
using InvenireServer.Domain.Core.Interfaces.Email;

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

        await using var context = _app.GetDatabaseContext();
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

        (await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeDto())).StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var dto = new LoginEmployeeDto
        {
            EmailAddress = employee.EmailAddress,
            Password = employee.Password
        };

        var response = await _client.PostAsJsonAsync("/api/auth/employee/login", dto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<LoginEmployeeResponseDto>();
        body.Should().NotBeNull();

        // Assert that the JWT has all the necessary claims.
        var jwt = Jwt.Parse(body!.Token);
        jwt.Payload.Should().Contain(c => c.Type == "role");
        jwt.Payload.Should().Contain(c => c.Type == "employee_id" && c.Value == employee.Id.ToString());
    }

    [Fact]
    public async Task LoginEmployee_Returns401WhenBadCredentials()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();

        (await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeDto())).StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var dto = new LoginEmployeeDto
        {
            EmailAddress = employee.EmailAddress,
            Password = new Faker().Internet.SecurePassword() // Generate a random password.
        };

        var response = await _client.PostAsJsonAsync("/api/auth/employee/login", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SendEmailVerification_Returns204AndEmailIsSentWithTheACorrectLink()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();
        var jwt = new JwtFaker().Create([
            new("role", JwtFactory.Policies.EMPLOYEE),
            new("employee_id", employee.Id.ToString())
        ]);

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {jwt.Write()}");

        (await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeDto())).StatusCode.Should().Be(HttpStatusCode.Created);

        // Act & Assert.
        var response = await _client.PostAsJsonAsync("/api/auth/employee/email-verification/send", new object());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var email = (EmailSenderFaker)_app.Services.GetRequiredService<IEmailSender>();
        email.CapturedMessages.Count.Should().Be(1);

        // Assert that the email message is properly constructed.
        var message = email.CapturedMessages[0];
        message.To.Should().ContainSingle(t => t.Address == employee.EmailAddress);
        message.Subject.Should().Contain("verify your email");
        message.Body.Should().NotBeNull();
        // Assert that the email body contains a verification link with a valid JWT that includes all required claims.
        message.Body.Should().MatchRegex(@"https?:\/\/[^\/]+\/verify-email\?token=([\w\-_.]+)");
        var match = Regex.Match(message.Body, @"https?:\/\/[^\/]+\/verify-email\?token=([\w\-_.]+)");
        var token = Jwt.Parse(match.Groups[1].Value);
        token.Payload.Should().Contain(c => c.Type == "role" && c.Value == nameof(Employee).ToUpper());
        token.Payload.Should().Contain(c => c.Type == "employee_id" && c.Value == employee.Id.ToString());
        token.Payload.Should().Contain(c => c.Type == "email_verification" && c.Value == bool.TrueString);
    }

    [Fact]
    public async Task ConfirmEmailVerification_Returns204AndEmailIsVerified()
    {
        // Prepare.
        var employee = new EmployeeFaker().Generate();
        var jwt = new JwtFaker().Create([
            new("role", JwtFactory.Policies.EMPLOYEE),
            new("employee_id", employee.Id.ToString())
        ]);

        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {jwt.Write()}");

        (await _client.PostAsJsonAsync("/api/auth/employee/register", employee.ToRegisterEmployeeDto())).StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/auth/employee/email-verification/send", new object())).StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Extract the token from the verification link and call the backend endpoint directly.
        var email = (EmailSenderFaker)_app.Services.GetRequiredService<IEmailSender>();
        email.CapturedMessages.Count.Should().Be(1);
        var match = Regex.Match(email.CapturedMessages[0].Body, @"https?:\/\/[^\/]+\/verify-email\?token=([\w\-_.]+)");
        var token = Jwt.Parse(match.Groups[1].Value);
        _client.DefaultRequestHeaders.Remove("Authorization");
        _client.DefaultRequestHeaders.Add("Authorization", $"BEARER {token.Write()}");

        // Act & Assert.
        var response = await _client.PostAsJsonAsync($"/api/auth/employee/email-verification/confirm", new object());
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await using var context = _app.GetDatabaseContext();
        var updatedEmployee = await context.Employees.FirstOrDefaultAsync(e => e.Id == employee.Id);

        // Assert that the employee has a verified email.
        updatedEmployee!.IsEmailAddressVerified.Should().Be(true);
    }
}
