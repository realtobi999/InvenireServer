using InvenireServer.Application.Core.Admins.Commands.Delete;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Admins.Commands;

public class DeleteAdminCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly DeleteAdminCommandHandler _handler;

    public DeleteAdminCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new DeleteAdminCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_DeletesAdminCorrectly()
    {
        // Prepare.
        var admin = AdminFaker.Fake();

        var command = new DeleteAdminCommand
        {
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync((Organization?)null);
        _services.Setup(s => s.Admins.DeleteAsync(It.IsAny<Admin>()));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenOrganizationIsNotDeleted()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin);

        var command = new DeleteAdminCommand
        {
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization); // Return the organization.
        _services.Setup(s => s.Admins.DeleteAsync(It.IsAny<Admin>()));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The organization must be deleted before the admin can be removed.");
    }
}
