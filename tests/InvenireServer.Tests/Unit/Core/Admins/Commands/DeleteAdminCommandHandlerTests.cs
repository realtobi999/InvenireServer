using InvenireServer.Application.Core.Admins.Commands.Delete;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Admins.Commands;

public class DeleteAdminCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly DeleteAdminCommandHandler _handler;

    public DeleteAdminCommandHandlerTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _handler = new DeleteAdminCommandHandler(_repositories.Object);
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

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync((Organization?)null);
        _repositories.Setup(r => r.Admins.Delete(It.IsAny<Admin>()));
        _repositories.Setup(r => r.SaveOrThrowAsync());

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

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Admins.Delete(It.IsAny<Admin>()));
        _repositories.Setup(r => r.SaveOrThrowAsync());

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The organization must be deleted before the admin can be removed.");
    }
}
