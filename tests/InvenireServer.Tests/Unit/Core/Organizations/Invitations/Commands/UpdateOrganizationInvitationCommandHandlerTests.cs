using InvenireServer.Application.Core.Organizations.Invitations.Commands.Update;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Organizations.Invitations.Commands;

/// <summary>
/// Tests for <see cref="UpdateOrganizationInvitationCommandHandler"/>.
/// </summary>
public class UpdateOrganizationInvitationCommandHandlerTests : CommandHandlerTester
{
    private readonly UpdateOrganizationInvitationCommandHandler _handler;

    public UpdateOrganizationInvitationCommandHandlerTests()
    {
        _handler = new UpdateOrganizationInvitationCommandHandler(_repositories.Object);
    }

    /// <summary>
    /// Verifies that the handler updates the invitation description.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, invitations: [invitation]);
        var command = new UpdateOrganizationInvitationCommand
        {
            Description = _faker.Lorem.Sentence(),
            Jwt = new Jwt([], []),
            InvitationId = invitation.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);
        _repositories.Setup(r => r.Organizations.Invitations.ExecuteUpdateAsync(invitation)).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        invitation.Description.Should().Be(command.Description);
    }

    /// <summary>
    /// Verifies that the handler throws when the admin is not found.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenAdminIsNotFound()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new UpdateOrganizationInvitationCommand
        {
            Description = _faker.Lorem.Sentence(),
            Jwt = new Jwt([], []),
            InvitationId = Guid.NewGuid(),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found in the system");
    }

    /// <summary>
    /// Verifies that the handler throws when the admin does not own an organization.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenOrganizationIsNotCreated()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new UpdateOrganizationInvitationCommand
        {
            Description = _faker.Lorem.Sentence(),
            Jwt = new Jwt([], []),
            InvitationId = Guid.NewGuid(),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin doesn't own a organization.");
    }

    /// <summary>
    /// Verifies that the handler throws when the invitation is not found.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenInvitationIsNotFound()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin);
        var command = new UpdateOrganizationInvitationCommand
        {
            Description = _faker.Lorem.Sentence(),
            Jwt = new Jwt([], []),
            InvitationId = Guid.NewGuid(),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync((OrganizationInvitation?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The invitation was not found in the system.");
    }

    /// <summary>
    /// Verifies that the handler throws when the invitation is not part of the organization.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenInvitationIsNotPartOfOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var invitation = OrganizationInvitationFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin);
        var command = new UpdateOrganizationInvitationCommand
        {
            Description = _faker.Lorem.Sentence(),
            Jwt = new Jwt([], []),
            InvitationId = invitation.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Invitations.GetAsync(i => i.Id == command.InvitationId)).ReturnsAsync(invitation);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("The invitation isn't part of the organization.");
    }
}
