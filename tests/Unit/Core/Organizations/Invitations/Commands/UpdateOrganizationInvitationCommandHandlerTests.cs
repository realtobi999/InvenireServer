using InvenireServer.Application.Core.Organizations.Invitations.Commands.Update;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Organizations.Invitations.Commands;

public class UpdateOrganizationInvitationCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly UpdateOrganizationInvitationCommandHandler _handler;

    public UpdateOrganizationInvitationCommandHandlerTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _handler = new UpdateOrganizationInvitationCommandHandler(_repositories.Object);
    }

    [Fact]
    public async Task Handle_UpdatesInvitationCorrectly()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, invitations: [invitation]);

        var command = new UpdateOrganizationInvitationCommand
        {
            Jwt = new Jwt([], []),
            Description = new Faker().Lorem.Sentences(),
            InvitationId = invitation.Id,
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);
        _repositories.Setup(r => r.Organizations.Invitations.Update(It.IsAny<OrganizationInvitation>()));
        _repositories.Setup(r => r.SaveOrThrowAsync());

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        // Assert that the invitation is correctly updated.
        invitation.Description.Should().Be(command.Description);
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheAdminIsNotFound()
    {
        // Prepare.
        var invitation = OrganizationInvitationFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: null, invitations: [invitation]);

        var command = new UpdateOrganizationInvitationCommand
        {
            Jwt = new Jwt([], []),
            Description = new Faker().Lorem.Sentences(),
            InvitationId = invitation.Id,
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenAdminDoesntOwnAnOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: null, invitations: [invitation]);

        var command = new UpdateOrganizationInvitationCommand
        {
            Jwt = new Jwt([], []),
            Description = new Faker().Lorem.Sentences(),
            InvitationId = invitation.Id,
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync((Organization?)null);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin doesn't own a organization.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheInvitationIsNotFound()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, invitations: []);

        var command = new UpdateOrganizationInvitationCommand
        {
            Jwt = new Jwt([], []),
            Description = new Faker().Lorem.Sentences(),
            InvitationId = OrganizationInvitationFaker.Fake().Id,
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync((OrganizationInvitation?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The invitation was not found in the system");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenInvitationIsNotPartOfOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, invitations: []);

        var command = new UpdateOrganizationInvitationCommand
        {
            Jwt = new Jwt([], []),
            Description = new Faker().Lorem.Sentences(),
            InvitationId = invitation.Id,
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("The invitation isn't part of the organization.");
    }
}
