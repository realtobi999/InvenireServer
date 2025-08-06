using InvenireServer.Application.Core.Organizations.Invitations.Commands.Update;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Organizations.Invitations.Commands;

public class UpdateOrganizationInvitationCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly UpdateOrganizationInvitationCommandHandler _handler;

    public UpdateOrganizationInvitationCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new UpdateOrganizationInvitationCommandHandler(_services.Object);
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
            Description = new Faker().Lorem.Sentences(),
            Jwt = new Jwt([], []),
            InvitationId = invitation.Id,
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);
        _services.Setup(s => s.Organizations.Invitations.UpdateAsync(It.IsAny<OrganizationInvitation>()));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().NotThrowAsync();

        // Assert that the invitation is correctly updated.
        invitation.Description.Should().Be(command.Description);
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
            Description = new Faker().Lorem.Sentences(),
            Jwt = new Jwt([], []),
            InvitationId = invitation.Id,
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("You have not created an organization.");
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
            Description = new Faker().Lorem.Sentences(),
            Jwt = new Jwt([], []),
            InvitationId = invitation.Id,
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);
        _services.Setup(s => s.Organizations.Invitations.UpdateAsync(It.IsAny<OrganizationInvitation>()));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("The invitation is not part of your organization.");
    }
}
