using InvenireServer.Application.Core.Properties.Commands.Delete;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Properties;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Properties.Commands;

public class DeletePropertyCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly DeletePropertyCommandHandler _handler;

    public DeletePropertyCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new DeletePropertyCommandHandler(_services.Object);
    }

    public async Task Handle_DeletesPropertyCorrectly()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, property: property);

        var command = new DeletePropertyCommand
        {
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.DeleteAsync(It.IsAny<Property>()));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().NotThrowAsync();
    }

    public async Task Handle_ThrowsExceptionWhenAdminDoesntOwnAnOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: null, property: property);

        var command = new DeletePropertyCommand
        {
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>("You do not own a organization.");
    }

    public async Task Handle_ThrowsExceptionWhenPropertyIsNotCreated()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, property: null);

        var command = new DeletePropertyCommand
        {
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>("You have not created a property.");
    }
}
