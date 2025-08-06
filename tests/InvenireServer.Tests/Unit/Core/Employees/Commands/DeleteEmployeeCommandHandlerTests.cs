using InvenireServer.Application.Core.Employees.Commands.Delete;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Tests.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Employees.Commands;

public class DeleteEmployeeCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly DeleteEmployeeCommandHandler _handler;

    public DeleteEmployeeCommandHandlerTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _handler = new DeleteEmployeeCommandHandler(_repositories.Object);
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

        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Employees.Delete(It.IsAny<Employee>()));
        _repositories.Setup(r => r.SaveOrThrowAsync());

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().NotThrowAsync();
    }
}
