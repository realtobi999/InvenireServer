using InvenireServer.Application.Core.Admins.Commands.Delete;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Admins.Commands;

public class DeleteAdminCommandHandlerTests : CommandHandlerTester
{
    private readonly DeleteAdminCommandHandler _handler;

    public DeleteAdminCommandHandlerTests()
    {
        _handler = new DeleteAdminCommandHandler(_repositories.Object);
    }

    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new DeleteAdminCommand
        {
            Jwt = _jwt.Builder.Build([])
        };

        // Prepare - repository.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync((Organization?)null);
        _repositories.Setup(r => r.Admins.ExecuteDeleteAsync(admin)).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenAdminIsNotFound()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new DeleteAdminCommand
        {
            Jwt = _jwt.Builder.Build([])
        };

        // Prepare - repository.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenOrganizationIsNotDeleted()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin);
        var command = new DeleteAdminCommand
        {
            Jwt = _jwt.Builder.Build([])
        };

        // Prepare - repository.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Conflict409Exception>().WithMessage("The admin's organization must be deleted before the admin can be removed.");
    }
}
