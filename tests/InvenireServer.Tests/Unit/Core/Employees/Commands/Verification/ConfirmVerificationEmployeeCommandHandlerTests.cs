using InvenireServer.Application.Core.Employees.Commands.Verification.Confirm;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Employees.Commands.Verification;

/// <summary>
/// Tests for <see cref="ConfirmVerificationEmployeeCommandHandler"/>.
/// </summary>
public class ConfirmVerificationEmployeeCommandHandlerTests : CommandHandlerTester
{
    private readonly ConfirmVerificationEmployeeCommandHandler _handler;

    public ConfirmVerificationEmployeeCommandHandlerTests()
    {
        _handler = new ConfirmVerificationEmployeeCommandHandler(_repositories.Object);
    }

    /// <summary>
    /// Verifies that the handler confirms verification when the token purpose is valid.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new ConfirmVerificationEmployeeCommand
        {
            Jwt = _jwt.Builder.Build([new("purpose", "email_verification")])
        };

        employee.IsVerified = false;

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Employees.ExecuteUpdateAsync(employee)).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        employee.IsVerified.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that the handler throws when the token purpose is missing.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenTokenIsMissingPurpose()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new ConfirmVerificationEmployeeCommand
        {
            Jwt = _jwt.Builder.Build([])
        };

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The token's purpose is missing.");
    }

    /// <summary>
    /// Verifies that the handler throws when the token purpose is invalid.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenTokenHasInvalidPurpose()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new ConfirmVerificationEmployeeCommand
        {
            Jwt = _jwt.Builder.Build([new("purpose", "invalid")])
        };

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("The token's purpose is not for email verification.");
    }

    /// <summary>
    /// Verifies that the handler throws when the employee is not found.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenEmployeeIsNotFound()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new ConfirmVerificationEmployeeCommand
        {
            Jwt = _jwt.Builder.Build([new("purpose", "email_verification")])
        };

        employee.IsVerified = false;

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The employee was not found in the system.");
    }
}
