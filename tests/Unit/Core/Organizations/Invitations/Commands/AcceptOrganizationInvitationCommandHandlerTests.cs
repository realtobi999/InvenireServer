using InvenireServer.Application.Core.Organizations.Invitations.Commands.Accept;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Organizations.Invitations.Commands;

public class AcceptOrganizationInvitationCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly AcceptOrganizationInvitationCommandHandler _handler;

    public AcceptOrganizationInvitationCommandHandlerTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _handler = new AcceptOrganizationInvitationCommandHandler(_repositories.Object);
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
            InvitationId = invitation.Id
        };

        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);
        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Id == invitation.OrganizationId)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Employees.Update(employee));
        _repositories.Setup(r => r.Organizations.Update(organization));
        _repositories.Setup(r => r.Organizations.Invitations.Delete(invitation));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        // Assert that the employee is part of the organization.
        employee.OrganizationId.Should().Be(organization.Id);
        organization.Employees.Should().Contain(e => e.Id == employee.Id);
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheEmployeeIsNotFound()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake(employee: EmployeeFaker.Fake());
        var organization = OrganizationFaker.Fake(admin: admin, invitations: [invitation]);

        var command = new AcceptOrganizationInvitationCommand
        {
            Jwt = new Jwt([], []),
            InvitationId = invitation.Id
        };

        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The employee was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheInvitationIsNotFound()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, invitations: []);

        var command = new AcceptOrganizationInvitationCommand
        {
            Jwt = new Jwt([], []),
            InvitationId = OrganizationInvitationFaker.Fake().Id
        };

        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync((OrganizationInvitation?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The invitation was not found in the system.");
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
            InvitationId = invitation.Id
        };

        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);
        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Id == invitation.OrganizationId)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The organization assigned to the invitation was not found in the system.");
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
            InvitationId = invitation.Id
        };

        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);
        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Id == invitation.OrganizationId)).ReturnsAsync(organization);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("The invitation is not assigned to the employee.");
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
            InvitationId = invitation.Id
        };

        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);
        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Id == invitation.OrganizationId)).ReturnsAsync(organization1);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The employee is already part of a another organization");
    }
}