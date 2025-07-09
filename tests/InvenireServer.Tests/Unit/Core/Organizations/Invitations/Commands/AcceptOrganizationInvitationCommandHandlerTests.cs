using InvenireServer.Application.Core.Organizations.Invitations.Commands.Accept;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Organizations.Invitations.Commands;

public class AcceptOrganizationInvitationCommandHandlerTests
{
    private readonly AcceptOrganizationInvitationCommandHandler _handler;
    private readonly Mock<IServiceManager> _services;

    public AcceptOrganizationInvitationCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new AcceptOrganizationInvitationCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_JoinsEmployeeToOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake(employee: employee);
        var organization = OrganizationFaker.Fake(admin: admin, invitations: [invitation]);

        var command = new AcceptOrganizationInvitationCommand
        {
            Jwt = new Jwt([], []),
            OrganizationId = organization.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.GetAsync(o => o.Id == command.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Organizations.Invitations.TryGetAsync(i => i.OrganizationId == organization.Id && i.Employee!.Id == employee.Id)).ReturnsAsync(invitation);
        _services.Setup(s => s.Organizations.Invitations.DeleteAsync(invitation));
        _services.Setup(s => s.Organizations.UpdateAsync(organization));
        _services.Setup(s => s.Employees.UpdateAsync(employee));

        // Act & Assert.
        await _handler.Handle(command, new CancellationToken());

        // Assert that the employee is part of the organization.
        employee.OrganizationId.Should().Be(organization.Id);
        organization.Employees.Should().Contain(e => e.Id == employee.Id);
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheInvitationDoesntBelowToTheProvidedOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake(employee: employee);
        var organization = OrganizationFaker.Fake(admin: admin, invitations: []);

        var command = new AcceptOrganizationInvitationCommand
        {
            Jwt = new Jwt([], []),
            OrganizationId = organization.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.GetAsync(o => o.Id == command.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Organizations.Invitations.TryGetAsync(i => i.OrganizationId == organization.Id && i.Employee!.Id == employee.Id)).ReturnsAsync((OrganizationInvitation?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("There is no invitation for you to join this organization.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheInvitationDoesntBelowToTheProvidedEmployee()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake(employee: EmployeeFaker.Fake());
        var organization = OrganizationFaker.Fake(admin: admin, invitations: [invitation]);

        var command = new AcceptOrganizationInvitationCommand
        {
            Jwt = new Jwt([], []),
            OrganizationId = organization.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.GetAsync(o => o.Id == command.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Organizations.Invitations.TryGetAsync(i => i.OrganizationId == organization.Id && i.Employee!.Id == employee.Id)).ReturnsAsync((OrganizationInvitation?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("There is no invitation for you to join this organization.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheEmployeeIsAlreadyAPartOfOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake(employee: employee);
        var organization1 = OrganizationFaker.Fake(admin: admin, invitations: [invitation]);
        var organization2 = OrganizationFaker.Fake(employees: [employee]);

        var command = new AcceptOrganizationInvitationCommand
        {
            Jwt = new Jwt([], []),
            OrganizationId = organization1.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.GetAsync(o => o.Id == command.OrganizationId)).ReturnsAsync(organization1);
        _services.Setup(s => s.Organizations.Invitations.TryGetAsync(i => i.OrganizationId == organization1.Id && i.Employee!.Id == employee.Id)).ReturnsAsync(invitation);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("This employee is already part of a another organization");
    }
}