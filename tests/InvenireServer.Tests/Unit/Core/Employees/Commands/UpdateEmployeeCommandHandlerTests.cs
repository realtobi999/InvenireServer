using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Tests.Integration.Fakers.Users;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Core.Employees.Commands.Update;

namespace InvenireServer.Tests.Unit.Core.Employees.Commands;

public class UpdateEmployeeCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly UpdateEmployeeCommandHandler _handler;

    public UpdateEmployeeCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new UpdateEmployeeCommandHandler(_services.Object);
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

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Employees.UpdateAsync(It.IsAny<Employee>()));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().NotThrowAsync();

        // Assert that the employee is correctly updated.
        employee.Name.Should().Be(command.Name);
    }
}
