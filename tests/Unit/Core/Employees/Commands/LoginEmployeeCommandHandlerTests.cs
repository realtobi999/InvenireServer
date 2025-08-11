using InvenireServer.Application.Core.Employees.Commands.Login;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Tests.Fakers.Common;
using InvenireServer.Tests.Fakers.Users;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Tests.Unit.Core.Employees.Commands;

public class LoginEmployeeCommandHandlerTests
{
    private readonly PasswordHasher<Employee> _hasher;
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly LoginEmployeeCommandHandler _handler;

    public LoginEmployeeCommandHandlerTests()
    {
        _hasher = new PasswordHasher<Employee>();
        _repositories = new Mock<IRepositoryManager>();
        _handler = new LoginEmployeeCommandHandler(JwtManagerFaker.Initiate(), _hasher, _repositories.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectToken()
    {
        // Prepare
        var employee = EmployeeFaker.Fake();

        var command = new LoginEmployeeCommand
        {
            EmailAddress = employee.EmailAddress,
            Password = employee.Password
        };

        employee.IsVerified = true;
        employee.Password = _hasher.HashPassword(employee, employee.Password);

        _repositories.Setup(r => r.Employees.GetAsync(e => e.EmailAddress == command.EmailAddress)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Employees.Update(It.IsAny<Employee>()));
        _repositories.Setup(r => r.SaveOrThrowAsync());

        // Act & Assert
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert that the token has all the necessary claims.
        var jwt = JwtBuilder.Parse(result.Token);
        jwt.Payload.Should().Contain(c => c.Type == "role" && c.Value == Jwt.Roles.EMPLOYEE);
        jwt.Payload.Should().Contain(c => c.Type == "employee_id" && c.Value == employee.Id.ToString());
        jwt.Payload.Should().Contain(c => c.Type == "is_verified" && c.Value == bool.TrueString);

        // Assert that the employee has updated his last login timestamp.
        employee.LastLoginAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenEmployeeIsNotFound()
    {
        // Prepare
        var employee = EmployeeFaker.Fake();

        var command = new LoginEmployeeCommand
        {
            EmailAddress = new string([.. employee.EmailAddress.Reverse()]), // Set invalid email.
            Password = employee.Password
        };

        employee.IsVerified = true;
        employee.Password = _hasher.HashPassword(employee, employee.Password);

        _repositories.Setup(r => r.Employees.GetAsync(e => e.EmailAddress == command.EmailAddress)).ThrowsAsync(new NotFound404Exception());

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenIncorrectPassword()
    {
        // Prepare
        var employee = EmployeeFaker.Fake();

        var command = new LoginEmployeeCommand
        {
            EmailAddress = employee.EmailAddress,
            Password = new string([.. employee.Password.Reverse()]) // Set incorrect password
        };

        employee.IsVerified = true;
        employee.Password = _hasher.HashPassword(employee, employee.Password);

        _repositories.Setup(r => r.Employees.GetAsync(e => e.EmailAddress == command.EmailAddress)).ReturnsAsync(employee);

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenEmployeeIsNotVerified()
    {
        // Prepare
        var employee = EmployeeFaker.Fake();

        var command = new LoginEmployeeCommand
        {
            EmailAddress = employee.EmailAddress,
            Password = employee.Password
        };

        employee.IsVerified = false; // Set the employee as unverified.
        employee.Password = _hasher.HashPassword(employee, employee.Password);

        _repositories.Setup(r => r.Employees.GetAsync(e => e.EmailAddress == command.EmailAddress)).ReturnsAsync(employee);

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("Verification is required to proceed with login.");
    }
}