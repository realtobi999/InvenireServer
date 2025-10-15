using InvenireServer.Application.Core.Employees.Commands.Update;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Employees.Commands;

public class UpdateEmployeeCommandHandlerTests : CommandHandlerTester
{
    private readonly UpdateEmployeeCommandHandler _handler;

    public UpdateEmployeeCommandHandlerTests()
    {
        _handler = new UpdateEmployeeCommandHandler(_repositories.Object);
    }

    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new UpdateEmployeeCommand
        {
            FirstName = employee.FirstName.Reverse().ToString()!,
            LastName = employee.LastName.Reverse().ToString()!,
            Jwt = _jwt.Builder.Build([])
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Employees.ExecuteUpdateAsync(employee)).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        employee.FirstName.Should().Be(command.FirstName);
        employee.LastName.Should().Be(command.LastName);
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenEmployeeIsNotFound()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new UpdateEmployeeCommand
        {
            FirstName = employee.FirstName.Reverse().ToString()!,
            LastName = employee.LastName.Reverse().ToString()!,
            Jwt = _jwt.Builder.Build([])
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The employee was not found in the system.");
    }
}
