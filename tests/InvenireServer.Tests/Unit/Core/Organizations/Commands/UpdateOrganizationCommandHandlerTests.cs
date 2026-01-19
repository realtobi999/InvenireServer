using InvenireServer.Application.Core.Organizations.Commands.Update;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Organizations.Commands;

/// <summary>
/// Tests for <see cref="UpdateOrganizationCommandHandler"/>.
/// </summary>
public class UpdateOrganizationCommandHandlerTests : CommandHandlerTester
{
    private readonly UpdateOrganizationCommandHandler _handler;

    public UpdateOrganizationCommandHandlerTests()
    {
        _handler = new UpdateOrganizationCommandHandler(_repositories.Object);
    }

    /// <summary>
    /// Verifies that the handler updates the organization's name.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var command = new UpdateOrganizationCommand
        {
            Jwt = new Jwt([], []),
            Name = _faker.Lorem.Word()
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.ExecuteUpdateAsync(organization));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        organization.Name.Should().Be(command.Name);
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
        var command = new UpdateOrganizationCommand
        {
            Jwt = new Jwt([], []),
            Name = _faker.Lorem.Word()
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found in the system.");
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
        var organization = OrganizationFaker.Fake();
        var command = new UpdateOrganizationCommand
        {
            Jwt = new Jwt([], []),
            Name = _faker.Lorem.Word()
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin doesn't own a organization.");
    }
}
