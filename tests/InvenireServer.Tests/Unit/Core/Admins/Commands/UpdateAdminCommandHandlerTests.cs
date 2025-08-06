using InvenireServer.Application.Core.Admins.Commands.Update;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Tests.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Admins.Commands;

public class UpdateAdminCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly UpdateAdminCommandHandler _handler;

    public UpdateAdminCommandHandlerTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _handler = new UpdateAdminCommandHandler(_repositories.Object);
    }

    [Fact]
    public async Task Handle_UpdatesAdminCorrectly()
    {
        // Prepare.
        var admin = AdminFaker.Fake();

        var command = new UpdateAdminCommand
        {
            Jwt = new Jwt([], []),
            Name = new Faker().Lorem.Sentence()
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Admins.Update(It.IsAny<Admin>()));
        _repositories.Setup(r => r.SaveOrThrowAsync());

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().NotThrowAsync();

        // Assert that the admin is correctly updated.
        admin.Name.Should().Be(command.Name);
    }
}
