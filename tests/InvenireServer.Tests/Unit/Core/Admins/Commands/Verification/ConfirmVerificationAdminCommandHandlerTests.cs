using InvenireServer.Application.Core.Admins.Commands.Verification.Confirm;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Admins.Commands.Verification;

/// <summary>
/// Tests for <see cref="ConfirmVerificationAdminCommandHandler"/>.
/// </summary>
public class ConfirmVerificationAdminCommandHandlerTests : CommandHandlerTester
{
    private readonly ConfirmVerificationAdminCommandHandler _handler;

    public ConfirmVerificationAdminCommandHandlerTests()
    {
        _handler = new ConfirmVerificationAdminCommandHandler(_repositories.Object);
    }

    /// <summary>
    /// Verifies that the handler confirms verification when the token purpose is valid.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new ConfirmVerificationAdminCommand
        {
            Jwt = _jwt.Builder.Build([new("purpose", "email_verification")])
        };

        admin.IsVerified = false;

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Admins.ExecuteUpdateAsync(admin)).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        admin.IsVerified.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that the handler throws when the token purpose is missing.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenTokenIsMissingPurpose()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new ConfirmVerificationAdminCommand
        {
            Jwt = _jwt.Builder.Build([])
        };

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The token's purpose is missing.");
    }

    /// <summary>
    /// Verifies that the handler throws when the token purpose is invalid.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenTokenHasInvalidPurpose()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new ConfirmVerificationAdminCommand
        {
            Jwt = _jwt.Builder.Build([new("purpose", "invalid")])
        };

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("The token's purpose is not for email verification.");
    }

    /// <summary>
    /// Verifies that the handler throws when the admin is not found.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenAdminIsNotFound()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new ConfirmVerificationAdminCommand
        {
            Jwt = _jwt.Builder.Build([new("purpose", "email_verification")])
        };

        admin.IsVerified = false;

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found in the system.");
    }
}
