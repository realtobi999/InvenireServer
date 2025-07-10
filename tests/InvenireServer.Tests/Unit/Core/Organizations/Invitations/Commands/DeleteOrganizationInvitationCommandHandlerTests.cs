using InvenireServer.Application.Core.Organizations.Invitations.Commands.Delete;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Organizations.Invitations.Commands;

public class DeleteOrganizationInvitationCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly DeleteOrganizationInvitationCommandHandler _handler;

    public DeleteOrganizationInvitationCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new DeleteOrganizationInvitationCommandHandler(_services.Object);
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
            Jwt = new Jwt([], []),
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Organizations.Invitations.TryGetAsync(i => i.Id == command.Id)).ReturnsAsync(invitation);
        _services.Setup(s => s.Organizations.Invitations.DeleteAsync(invitation));
        _services.Setup(s => s.Organizations.UpdateAsync(organization));

        // Act & Assert.
        await _handler.Handle(command, new CancellationToken());

        // Assert that the organization is missing the invitation.
        organization.Invitations.Should().NotContain(i => i.Id == invitation.Id);
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
            Jwt = new Jwt([], []),
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("You have not created an organization. You must first create an organization before deleting any invitations.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheInvitationDoesntExist()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, invitations: []);

        var command = new DeleteOrganizationInvitationCommand
        {
            Id = invitation.Id,
            Jwt = new Jwt([], []),
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Organizations.Invitations.TryGetAsync(i => i.Id == command.Id)).ReturnsAsync((OrganizationInvitation?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The specified invitation does not exist or may have already been deleted.");
    }
}
