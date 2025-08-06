using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Application.Core.Employees.Commands.Update;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Tests.Unit.Core.Employees.Commands;

public class UpdateEmployeeCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly UpdateEmployeeCommandHandler _handler;

    public UpdateEmployeeCommandHandlerTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _handler = new UpdateEmployeeCommandHandler(_repositories.Object);
    }

    [Fact]
    public async Task Handle_UpdatesEmployeeCorrectly()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();

        var command = new UpdateEmployeeCommand
        {
            Name = new Faker().Lorem.Sentence(),
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Employees.Update(It.IsAny<Employee>()));
        _repositories.Setup(r => r.SaveOrThrowAsync());

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().NotThrowAsync();

        // Assert that the employee is correctly updated.
        employee.Name.Should().Be(command.Name);
    }
}
