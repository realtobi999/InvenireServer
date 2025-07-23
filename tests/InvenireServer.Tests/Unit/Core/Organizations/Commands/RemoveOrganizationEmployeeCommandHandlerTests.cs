using InvenireServer.Tests.Integration.Fakers.Users;
using InvenireServer.Application.Core.Organizations.Commands.Employee.Remove;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Organizations;

namespace InvenireServer.Tests.Unit.Core.Organizations.Commands;

public class RemoveEmployeeOrganizationCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly RemoveEmployeeOrganizationCommandHandler _handler;

    public RemoveEmployeeOrganizationCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new RemoveEmployeeOrganizationCommandHandler(_services.Object);
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

        _services.Setup(s => s.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync(employee);
        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);
        _services.Setup(s => s.Organizations.UpdateAsync(It.IsAny<Organization>()));

        // Act & Assert.
        await _handler.Handle(command, CancellationToken.None);

        // Assert that the organization is missing the removed employee.
        organization.Employees.Should().NotContain(e => e.Id == employee.Id);
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

        _services.Setup(s => s.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync(employee);
        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("You have not created an organization.");
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

        _services.Setup(s => s.Employees.GetAsync(e => e.Id == command.EmployeeId)).ReturnsAsync(employee);
        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("This employee is not a part of this organization.");
    }
}
