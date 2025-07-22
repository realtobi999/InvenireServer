using InvenireServer.Application.Core.Admins.Commands.Update;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Admins.Commands;

public class UpdateAdminCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly UpdateAdminCommandHandler _handler;

    public UpdateAdminCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new UpdateAdminCommandHandler(_services.Object);
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

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Admins.UpdateAsync(It.IsAny<Admin>()));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().NotThrowAsync();

        // Assert that the admin is correctly updated.
        admin.Name.Should().Be(command.Name);
    }
}
