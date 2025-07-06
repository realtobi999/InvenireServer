using InvenireServer.Application.Core.Organizations.Invitations.Commands.Accept;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
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
        var organization = new OrganizationFaker().Generate();
        var admin = new AdminFaker(organization).Generate();
        var employee = new EmployeeFaker().Generate();
        var invitation = new OrganizationInvitationFaker(organization, employee).Generate();

        organization.Admin = admin;
        organization.Invitations.Add(invitation);

        var command = new AcceptOrganizationInvitationCommand
        {
            Jwt = new Jwt([], []),
            InvitationId = invitation.Id,
            OrganizationId = organization.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.GetAsync(o => o.Id == command.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);
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
        var organization = new OrganizationFaker().Generate();
        var admin = new AdminFaker(organization).Generate();
        var employee = new EmployeeFaker().Generate();
        var invitation = new OrganizationInvitationFaker(new OrganizationFaker().Generate(), employee).Generate(); // Assign to a different organization.

        var command = new AcceptOrganizationInvitationCommand
        {
            Jwt = new Jwt([], []),
            InvitationId = invitation.Id,
            OrganizationId = organization.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.GetAsync(o => o.Id == command.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The invitation does not belong to the specified organization.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheInvitationDoesntBelowToTheProvidedEmployee()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var admin = new AdminFaker(organization).Generate();
        var employee = new EmployeeFaker().Generate();
        var invitation = new OrganizationInvitationFaker(organization, new EmployeeFaker().Generate()).Generate(); // Assign to a different employee.

        var command = new AcceptOrganizationInvitationCommand
        {
            Jwt = new Jwt([], []),
            InvitationId = invitation.Id,
            OrganizationId = organization.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.GetAsync(o => o.Id == command.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<Unauthorized401Exception>();
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheEmployeeIsAlreadyAPartOfOrganization()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var admin = new AdminFaker(organization).Generate();
        var employee = new EmployeeFaker(new OrganizationFaker().Generate()).Generate(); // Assign to a different organization.
        var invitation = new OrganizationInvitationFaker(organization, employee).Generate();

        var command = new AcceptOrganizationInvitationCommand
        {
            Jwt = new Jwt([], []),
            InvitationId = invitation.Id,
            OrganizationId = organization.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.GetAsync(o => o.Id == command.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("Employee is already a part of an organization.");
    }
}