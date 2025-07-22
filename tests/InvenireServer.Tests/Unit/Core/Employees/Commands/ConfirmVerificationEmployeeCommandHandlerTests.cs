using System.Security.Claims;
using InvenireServer.Application.Core.Employees.Commands.Verification.Confirm;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Employees.Commands;

public class ConfirmVerificationEmployeeCommandHandlerTests
{
    private readonly ConfirmVerificationEmployeeCommandHandler _handler;
    private readonly Mock<IServiceManager> _services;

    public ConfirmVerificationEmployeeCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new ConfirmVerificationEmployeeCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_VerifiesEmployeeCorrectly()
    {
        // Prepare
        var employee = EmployeeFaker.Fake();

        employee.IsVerified = false;

        var command = new ConfirmVerificationEmployeeCommand
        {
            Jwt = new Jwt([], [new Claim("purpose", "email_verification")])
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Employees.UpdateAsync(employee));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().NotThrowAsync();

        // Assert that the employee is verified.
        employee.IsVerified.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenPurposeClaimIsMissing()
    {
        // Prepare
        var command = new ConfirmVerificationEmployeeCommand
        {
            Jwt = new Jwt([], []) // Insert empty token.
        };

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The token's purpose is missing.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenPurposeClaimIsInvalid()
    {
        // Prepare
        var command = new ConfirmVerificationEmployeeCommand
        {
            Jwt = new Jwt([], [new Claim("purpose", "invalid")]) // Insert invalid purpose claim.
        };

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("The token's purpose is not for email verification.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenEmployeeIsAlreadyVerified()
    {
        // Prepare
        var employee = EmployeeFaker.Fake();

        employee.IsVerified = true; // Set the employee as verified.

        var command = new ConfirmVerificationEmployeeCommand
        {
            Jwt = new Jwt([], [new Claim("purpose", "email_verification")])
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The employees's verification status is already confirmed.");
    }
}