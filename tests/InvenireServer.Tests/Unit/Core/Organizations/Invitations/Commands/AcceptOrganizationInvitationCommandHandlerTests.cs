using InvenireServer.Application.Core.Organizations.Invitations.Commands.Accept;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Organizations.Invitations.Commands;

public class AcceptOrganizationInvitationCommandHandlerTests : CommandHandlerTester
{
    private readonly AcceptOrganizationInvitationCommandHandler _handler;

    public AcceptOrganizationInvitationCommandHandlerTests()
    {
        _handler = new AcceptOrganizationInvitationCommandHandler(_repositories.Object);
    }

    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake(employee);
        var organization = OrganizationFaker.Fake(invitations: [invitation]);
        var command = new AcceptOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([]),
            InvitationId = invitation.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);
        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Id == invitation.OrganizationId)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Employees.Update(employee));
        _repositories.Setup(r => r.Organizations.Update(organization));
        _repositories.Setup(r => r.Organizations.Invitations.Delete(invitation));
        _repositories.Setup(r => r.SaveOrThrowAsync()).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        employee.OrganizationId.Should().Be(organization.Id);
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenEmployeeIsNotFound()
    {
        // Prepare.
        var invitation = OrganizationInvitationFaker.Fake();
        var command = new AcceptOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([]),
            InvitationId = invitation.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The employee was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenInvitationIsNotFound()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new AcceptOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([]),
            InvitationId = Guid.NewGuid(),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync((OrganizationInvitation?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The invitation was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenOrganizationIsNotFound()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake(employee);
        var command = new AcceptOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([]),
            InvitationId = invitation.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);
        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Id == invitation.OrganizationId)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The organization assigned to the invitation was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenInvitationIsNotAssignedToEmployee()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var otherEmployee = EmployeeFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake(otherEmployee);
        var organization = OrganizationFaker.Fake(invitations: [invitation]);
        var command = new AcceptOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([]),
            InvitationId = invitation.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);
        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Id == invitation.OrganizationId)).ReturnsAsync(organization);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("The invitation is not assigned to the employee.");
    }
}
