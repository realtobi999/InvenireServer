using InvenireServer.Application.Core.Admins.Commands.Update;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Admins.Commands;

public class UpdateAdminCommandHandlerTests : CommandHandlerTester<UpdateAdminCommandHandler>
{
    public UpdateAdminCommandHandlerTests() : base(repos =>
    {
        return new UpdateAdminCommandHandler(repos.Object);
    })
    { }

    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new UpdateAdminCommand
        {
            FirstName = admin.FirstName.Reverse().ToString()!,
            LastName = admin.LastName.Reverse().ToString()!,
            Jwt = _jwt.Builder.Build([])
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Admins.ExecuteUpdateAsync(admin)).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        admin.FirstName.Should().Be(command.FirstName);
        admin.LastName.Should().Be(command.LastName);
    }
}
