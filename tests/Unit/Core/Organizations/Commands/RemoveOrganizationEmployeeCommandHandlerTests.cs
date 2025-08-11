using InvenireServer.Application.Core.Organizations.Commands.Employee.Remove;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Organizations.Commands;

public class RemoveEmployeeOrganizationCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly RemoveEmployeeOrganizationCommandHandler _handler;

    public RemoveEmployeeOrganizationCommandHandlerTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _handler = new RemoveEmployeeOrganizationCommandHandler(_repositories.Object);
    }

    [Fact]
    public async Task Handle_RemovesEmployee()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, employees: [employee]);

        var command = new RemoveEmployeeOrganizationCommand
        {
            Jwt = new Jwt([], []),
            EmployeeId = employee.Id
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.Update(It.IsAny<Organization>()));
        _repositories.Setup(r => r.SaveOrThrowAsync());

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenAdminIsNotFound()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();

        var command = new RemoveEmployeeOrganizationCommand
        {
            Jwt = new Jwt([], []),
            EmployeeId = employee.Id
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheAdminDoesntOwnAnOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: null, employees: [employee]);

        var command = new RemoveEmployeeOrganizationCommand
        {
            Jwt = new Jwt([], []),
            EmployeeId = employee.Id
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin doesn't own a organization.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheEmployeeIsNotFound()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, employees: []);

        var command = new RemoveEmployeeOrganizationCommand
        {
            Jwt = new Jwt([], []),
            EmployeeId = EmployeeFaker.Fake().Id
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The employee was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheEmployeeDoesntBelongInTheOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, employees: []);

        var command = new RemoveEmployeeOrganizationCommand
        {
            Jwt = new Jwt([], []),
            EmployeeId = employee.Id
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync(employee);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("The employee isn't part of the organization.");
    }
}
