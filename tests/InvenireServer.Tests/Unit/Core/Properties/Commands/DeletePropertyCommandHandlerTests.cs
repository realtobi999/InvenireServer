using InvenireServer.Application.Core.Properties.Commands.Delete;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Properties;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Properties.Commands;

/// <summary>
/// Tests for <see cref="DeletePropertyCommandHandler"/>.
/// </summary>
public class DeletePropertyCommandHandlerTests : CommandHandlerTester
{
    private readonly DeletePropertyCommandHandler _handler;

    public DeletePropertyCommandHandlerTests()
    {
        _handler = new DeletePropertyCommandHandler(_repositories.Object);
    }

    /// <summary>
    /// Verifies that the handler deletes the property without errors.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var command = new DeletePropertyCommand
        {
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.ExecuteDeleteAsync(property)).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();
    }

    /// <summary>
    /// Verifies that the handler throws when the admin is not found.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenAdminIsNotFound()
    {
        // Prepare.
        var command = new DeletePropertyCommand
        {
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found in the system.");
    }

    /// <summary>
    /// Verifies that the handler throws when the admin has no organization.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenOrganizationIsNotCreated()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new DeletePropertyCommand
        {
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin doesn't own a organization.");
    }

    /// <summary>
    /// Verifies that the handler throws when the organization has no property.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenPropertyIsNotFound()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var command = new DeletePropertyCommand
        {
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The organization doesn't have a property.");
    }
}
