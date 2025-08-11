using InvenireServer.Application.Core.Organizations.Invitations.Commands.Delete;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Organizations.Invitations.Commands;

public class DeleteOrganizationInvitationCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly DeleteOrganizationInvitationCommandHandler _handler;

    public DeleteOrganizationInvitationCommandHandlerTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _handler = new DeleteOrganizationInvitationCommandHandler(_repositories.Object);
    }

    [Fact]
    public async Task Handle_DeletesInvitation()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, invitations: [invitation]);

        var command = new DeleteOrganizationInvitationCommand
        {
            Id = invitation.Id,
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.Id)).ReturnsAsync(invitation);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Update(organization));
        _repositories.Setup(r => r.Organizations.Invitations.Delete(invitation));
        _repositories.Setup(r => r.SaveOrThrowAsync());

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheAdminIsNotFound()
    {
        // Prepare.
        var invitation = OrganizationInvitationFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: null, invitations: [invitation]);

        var command = new DeleteOrganizationInvitationCommand
        {
            Id = invitation.Id,
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFound404Exception>("The admin was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheAdminDoesntOwnAnOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: null, invitations: [invitation]);

        var command = new DeleteOrganizationInvitationCommand
        {
            Id = invitation.Id,
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.Id)).ReturnsAsync(invitation);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin doesn't own a organization.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheInvitationIsNotFound()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, invitations: []);

        var command = new DeleteOrganizationInvitationCommand
        {
            Id = invitation.Id,
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(s => s.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(s => s.Organizations.Invitations.GetAsync(i => i.Id == command.Id)).ReturnsAsync((OrganizationInvitation?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The invitation was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheInvitationIsNotPartOfTheOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, invitations: []);

        var command = new DeleteOrganizationInvitationCommand
        {
            Id = invitation.Id,
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.Id)).ReturnsAsync(invitation);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Unauthorized401Exception>("The invitation isn't part of the organization.");
    }
}
