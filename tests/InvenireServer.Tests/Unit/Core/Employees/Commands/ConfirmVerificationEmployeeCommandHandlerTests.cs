using InvenireServer.Application.Core.Employees.Commands.Verification.Confirm;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Employees.Commands;

public class ConfirmVerificationEmployeeCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly ConfirmVerificationEmployeeCommandHandler _handler;

    public ConfirmVerificationEmployeeCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new ConfirmVerificationEmployeeCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_ReturnsVerifiedEmployee()
    {
        // Prepare
        var employee = new EmployeeFaker().Generate();
        employee.IsVerified = false;

        var command = new ConfirmVerificationEmployeeCommand
        {
            Jwt = new Jwt([], [new("purpose", "email_verification")])
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Employees.UpdateAsync(employee));

        // Act & Assert.
        await _handler.Handle(command, new CancellationToken());

        // Assert that the employee is verified.
        employee.IsVerified.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenInvalidToken()
    {
        // Prepare
        var command = new ConfirmVerificationEmployeeCommand
        {
            Jwt = new Jwt([], []) // Insert empty token.
        };

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("Indication that the token is used for email verification is missing.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenEmployeeIsAlreadyVerified()
    {
        // Prepare
        var employee = new EmployeeFaker().Generate();
        employee.IsVerified = true; // Set the employee as verified.

        var command = new ConfirmVerificationEmployeeCommand
        {
            Jwt = new Jwt([], [new("purpose", "email_verification")])
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("Employee is already verified.");
    }
}
