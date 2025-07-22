using InvenireServer.Application.Core.Employees.Commands.Delete;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Employees.Commands;

public class DeleteEmployeeCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly DeleteEmployeeCommandHandler _handler;

    public DeleteEmployeeCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new DeleteEmployeeCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_DeletesEmployeeCorrectly()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();

        var command = new DeleteEmployeeCommand
        {
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Employees.DeleteAsync(It.IsAny<Employee>()));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().NotThrowAsync();
    }
}
