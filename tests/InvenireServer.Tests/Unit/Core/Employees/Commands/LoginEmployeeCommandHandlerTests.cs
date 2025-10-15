using InvenireServer.Application.Core.Employees.Commands.Login;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Tests.Unit.Core.Employees.Commands;

public class LoginEmployeeCommandHandlerTests : CommandHandlerTester
{
    private readonly PasswordHasher<Employee> _hasher;
    private readonly LoginEmployeeCommandHandler _handler;

    public LoginEmployeeCommandHandlerTests()
    {
        _hasher = new PasswordHasher<Employee>();
        _handler = new LoginEmployeeCommandHandler(_jwt, _hasher, _repositories.Object);
    }

    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new LoginEmployeeCommand
        {
            EmailAddress = employee.EmailAddress,
            Password = employee.Password,
        };

        // Hash the password before storing it, matching the format used in the database.
        employee.Password = _hasher.HashPassword(employee, employee.Password);
        // Set the employee as verified.
        employee.IsVerified = true;

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(a => a.EmailAddress == command.EmailAddress)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Employees.ExecuteUpdateAsync(employee)).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        employee.LastLoginAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

        var result = await action.Invoke();
        result.Should().NotBeNull();
        result.Token.Should().NotBeNull();
        result.Token.Payload.Should().NotBeEmpty();
        result.Token.Payload.Should().Contain(c => c.Type == "role" && c.Value == Jwt.Roles.EMPLOYEE);
        result.Token.Payload.Should().Contain(c => c.Type == "employee_id" && c.Value == employee.Id.ToString());
        result.Token.Payload.Should().Contain(c => c.Type == "is_verified" && c.Value == bool.TrueString);
        result.TokenString.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenEmployeeIsNotFound()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new LoginEmployeeCommand
        {
            EmailAddress = employee.EmailAddress,
            Password = employee.Password,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(a => a.EmailAddress == command.EmailAddress)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenEmployeeIsNotVerified()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new LoginEmployeeCommand
        {
            EmailAddress = employee.EmailAddress,
            Password = employee.Password,
        };

        // Set the employee as not verified.
        employee.IsVerified = false;

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(a => a.EmailAddress == command.EmailAddress)).ReturnsAsync(employee);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("Verification is required to proceed with login.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenPasswordsDontMatch()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new LoginEmployeeCommand
        {
            EmailAddress = employee.EmailAddress,
            Password = employee.Password.Reverse().ToString()!,
        };

        // Hash the password before storing it, matching the format used in the database.
        employee.Password = _hasher.HashPassword(employee, employee.Password);
        // Set the employee as verified.
        employee.IsVerified = true;

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(a => a.EmailAddress == command.EmailAddress)).ReturnsAsync(employee);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("Invalid credentials.");
    }
}
