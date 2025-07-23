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
            InvitationId = invitation.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == invitation.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Organizations.Invitations.DeleteAsync(invitation));
        _services.Setup(s => s.Organizations.UpdateAsync(organization));
        _services.Setup(s => s.Employees.UpdateAsync(employee));

        // Act & Assert.
        await _handler.Handle(command, CancellationToken.None);

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
            InvitationId = invitation.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == invitation.OrganizationId)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The organization assigned to the invitation was not found.");
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

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == invitation.OrganizationId)).ReturnsAsync(organization);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("The invitation is not assigned to you.");
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

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == invitation.OrganizationId)).ReturnsAsync(organization1);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("This employee is already part of a another organization");
    }
}