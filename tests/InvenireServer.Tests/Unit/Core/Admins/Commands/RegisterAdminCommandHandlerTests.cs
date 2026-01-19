using InvenireServer.Application.Core.Admins.Commands.Register;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Tests.Unit.Core.Admins.Commands;

/// <summary>
/// Tests for <see cref="RegisterAdminCommandHandler"/>.
/// </summary>
public class RegisterAdminCommandHandlerTests : CommandHandlerTester
{
    private readonly PasswordHasher<Admin> _hasher;
    private readonly RegisterAdminCommandHandler _handler;

    public RegisterAdminCommandHandlerTests()
    {
        _hasher = new PasswordHasher<Admin>();
        _handler = new RegisterAdminCommandHandler(_jwt, _hasher, _repositories.Object);
    }

    /// <summary>
    /// Verifies that the handler registers an admin and issues a token.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new RegisterAdminCommand
        {
            FirstName = admin.FirstName,
            LastName = admin.LastName,
            EmailAddress = admin.EmailAddress,
            Password = admin.Password,
            PasswordConfirm = admin.Password,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.ExecuteCreateAsync(admin)).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        var result = await action.Invoke();
        result.Admin.Should().NotBeNull();
        result.Admin.Id.Should().NotBeEmpty();
        result.Admin.FirstName.Should().Be(command.FirstName);
        result.Admin.LastName.Should().Be(command.LastName);
        result.Admin.EmailAddress.Should().Be(command.EmailAddress);
        result.Admin.Password.Should().NotBe(command.Password);
        result.Admin.IsVerified.Should().Be(false);
        result.Admin.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        result.Admin.LastLoginAt.Should().BeNull();
        result.Admin.LastUpdatedAt.Should().BeNull();
        result.Should().NotBeNull();
        result.Token.Should().NotBeNull();
        result.Token.Payload.Should().NotBeEmpty();
        result.Token.Payload.Should().Contain(c => c.Type == "role" && c.Value == Jwt.Roles.ADMIN);
        result.Token.Payload.Should().Contain(c => c.Type == "admin_id" && c.Value == result.Admin.Id.ToString());
        result.Token.Payload.Should().Contain(c => c.Type == "is_verified" && c.Value == bool.FalseString);
        result.TokenString.Should().NotBeNullOrEmpty();
    }
}
