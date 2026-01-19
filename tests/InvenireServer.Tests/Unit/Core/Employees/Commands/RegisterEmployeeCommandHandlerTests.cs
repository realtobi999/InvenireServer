using InvenireServer.Application.Core.Employees.Commands.Register;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Tests.Unit.Core.Employees.Commands;

/// <summary>
/// Tests for <see cref="RegisterEmployeeCommandHandler"/>.
/// </summary>
public class RegisterEmployeeCommandHandlerTests : CommandHandlerTester
{
    private readonly PasswordHasher<Employee> _hasher;
    private readonly RegisterEmployeeCommandHandler _handler;

    public RegisterEmployeeCommandHandlerTests()
    {
        _hasher = new PasswordHasher<Employee>();
        _handler = new RegisterEmployeeCommandHandler(_jwt, _hasher, _repositories.Object);
    }

    /// <summary>
    /// Verifies that the handler registers an employee and issues a token.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new RegisterEmployeeCommand
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            EmailAddress = employee.EmailAddress,
            Password = employee.Password,
            PasswordConfirm = employee.Password,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.ExecuteCreateAsync(employee)).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        var result = await action.Invoke();
        result.Employee.Should().NotBeNull();
        result.Employee.Id.Should().NotBeEmpty();
        result.Employee.FirstName.Should().Be(command.FirstName);
        result.Employee.LastName.Should().Be(command.LastName);
        result.Employee.EmailAddress.Should().Be(command.EmailAddress);
        result.Employee.Password.Should().NotBe(command.Password);
        result.Employee.IsVerified.Should().Be(false);
        result.Employee.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        result.Employee.LastLoginAt.Should().BeNull();
        result.Employee.LastUpdatedAt.Should().BeNull();
        result.Should().NotBeNull();
        result.Token.Should().NotBeNull();
        result.Token.Payload.Should().NotBeEmpty();
        result.Token.Payload.Should().Contain(c => c.Type == "role" && c.Value == Jwt.Roles.EMPLOYEE);
        result.Token.Payload.Should().Contain(c => c.Type == "employee_id" && c.Value == result.Employee.Id.ToString());
        result.Token.Payload.Should().Contain(c => c.Type == "is_verified" && c.Value == bool.FalseString);
        result.TokenString.Should().NotBeNullOrEmpty();
    }
}
