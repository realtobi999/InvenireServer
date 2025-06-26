using InvenireServer.Application.Core.Employees.Commands.Login;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Tests.Integration.Fakers.Common;
using InvenireServer.Tests.Integration.Fakers.Users;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Tests.Unit.Core.Employees.Commands;

public class LoginEmployeeCommandHandlerTests
{
    private readonly IJwtManager _jwt;
    private readonly Mock<IServiceManager> _services;
    private readonly PasswordHasher<Employee> _hasher;
    private readonly LoginEmployeeCommandHandler _handler;

    public LoginEmployeeCommandHandlerTests()
    {
        _jwt = new JwtManagerFaker().Initiate();
        _hasher = new PasswordHasher<Employee>();
        _services = new Mock<IServiceManager>();
        _handler = new LoginEmployeeCommandHandler(_services.Object, _hasher, _jwt);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectToken()
    {
        // Prepare
        var employee = new EmployeeFaker().Generate();

        var command = new LoginEmployeeCommand
        {
            EmailAddress = employee.EmailAddress,
            Password = employee.Password,
        };

        employee.IsVerified = true;
        employee.Password = _hasher.HashPassword(employee, employee.Password);

        _services.Setup(s => s.Employees.GetAsync(e => e.EmailAddress == command.EmailAddress)).ReturnsAsync(employee);

        // Act & Assert
        var result = await _handler.Handle(command, new CancellationToken());

        // Assert that the token has all the necessary claims.
        var jwt = JwtBuilder.Parse(result.Token);
        jwt.Payload.Should().Contain(c => c.Type == "role" && c.Value == Jwt.Roles.EMPLOYEE);
        jwt.Payload.Should().Contain(c => c.Type == "employee_id" && c.Value == employee.Id.ToString());
        jwt.Payload.Should().Contain(c => c.Type == "is_verified" && c.Value == bool.TrueString);

        // Assert that the employee has updated his last login timestamp.
        employee.LastLoginAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenEmployeeIsNotFound()
    {
        // Prepare
        var employee = new EmployeeFaker().Generate();

        var command = new LoginEmployeeCommand
        {
            EmailAddress = new([.. employee.EmailAddress.Reverse()]), // Set invalid email.
            Password = employee.Password,
        };

        employee.IsVerified = true;
        employee.Password = _hasher.HashPassword(employee, employee.Password);

        _services.Setup(s => s.Employees.GetAsync(e => e.EmailAddress == command.EmailAddress)).ThrowsAsync(new NotFound404Exception());

        // Act & Assert
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenIncorrectPassword()
    {
        // Prepare
        var employee = new EmployeeFaker().Generate();

        var command = new LoginEmployeeCommand
        {
            EmailAddress = employee.EmailAddress,
            Password = new([.. employee.Password.Reverse()]), // Set incorrect password
        };

        employee.IsVerified = true;
        employee.Password = _hasher.HashPassword(employee, employee.Password);

        _services.Setup(s => s.Employees.GetAsync(e => e.EmailAddress == command.EmailAddress)).ReturnsAsync(employee);

        // Act & Assert
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenEmployeeIsNotVerified()
    {
        // Prepare
        var employee = new EmployeeFaker().Generate();

        var command = new LoginEmployeeCommand
        {
            EmailAddress = employee.EmailAddress,
            Password = employee.Password,
        };

        employee.IsVerified = false; // Set the employee as unverified.
        employee.Password = _hasher.HashPassword(employee, employee.Password);

        _services.Setup(s => s.Employees.GetAsync(e => e.EmailAddress == command.EmailAddress)).ReturnsAsync(employee);

        // Act & Assert
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("Verification required to proceed.");
    }
}
