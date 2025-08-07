using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Core.Organizations.Commands.Delete;

namespace InvenireServer.Tests.Unit.Core.Organizations.Commands;

public class DeleteOrganizationCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly DeleteOrganizationCommandHandler _handler;

    public DeleteOrganizationCommandHandlerTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _handler = new DeleteOrganizationCommandHandler(_repositories.Object);
    }

    [Fact]
    public async Task Handle_DeletesOrganizationCorrectly()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin);

        var command = new DeleteOrganizationCommand
        {
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Delete(It.IsAny<Organization>()));
        _repositories.Setup(r => r.SaveOrThrowAsync());

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenAdminIsNotFound()
    {
        // Prepare.
        var command = new DeleteOrganizationCommand
        {
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenAdminDoesntOwnAnOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: null);

        var command = new DeleteOrganizationCommand
        {
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(s => s.Organizations.GetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin doesn't own a organization.");
    }
}
