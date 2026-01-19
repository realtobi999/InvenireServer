using InvenireServer.Application.Core.Employees.Commands.Delete;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Employees.Commands;

/// <summary>
/// Tests for <see cref="DeleteEmployeeCommandHandler"/>.
/// </summary>
public class DeleteEmployeeCommandHandlerTests : CommandHandlerTester
{
    private readonly DeleteEmployeeCommandHandler _handler;

    public DeleteEmployeeCommandHandlerTests()
    {
        _handler = new DeleteEmployeeCommandHandler(_repositories.Object);
    }

    /// <summary>
    /// Verifies that the handler deletes the employee.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new DeleteEmployeeCommand
        {
            Jwt = _jwt.Builder.Build([])
        };

        // Prepare - repository.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Employees.ExecuteDeleteAsync(employee)).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();
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
        var command = new DeleteEmployeeCommand
        {
            Jwt = _jwt.Builder.Build([])
        };

        // Prepare - repository.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The employee was not found in the system.");
    }
}
