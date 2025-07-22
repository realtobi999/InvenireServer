using System.Security.Claims;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Users;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Core.Admins.Commands.Verification.Confirm;

namespace InvenireServer.Tests.Unit.Core.Admins.Commands;

public class ConfirmVerificationAdminCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly ConfirmVerificationAdminCommandHandler _handler;

    public ConfirmVerificationAdminCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new ConfirmVerificationAdminCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_VerifiesAdminCorrectly()
    {
        // Prepare
        var admin = AdminFaker.Fake();

        admin.IsVerified = false;

        var command = new ConfirmVerificationAdminCommand
        {
            Jwt = new Jwt([], [new Claim("purpose", "email_verification")])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Admins.UpdateAsync(admin));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().NotThrowAsync();

        // Assert that the admin is verified.
        admin.IsVerified.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenPurposeClaimIsMissing()
    {
        // Prepare
        var command = new ConfirmVerificationAdminCommand
        {
            Jwt = new Jwt([], []) // Insert a empty token.
        };

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The token's purpose is missing.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenPurposeClaimIsInvalid()
    {
        // Prepare
        var command = new ConfirmVerificationAdminCommand
        {
            Jwt = new Jwt([], [new Claim("purpose", "invalid")]) // Insert a invalid purpose claim.
        };

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("The token's purpose is not for email verification.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenAdminIsAlreadyVerified()
    {
        // Prepare
        var admin = AdminFaker.Fake();

        admin.IsVerified = true; // Set the admin as verified.

        var command = new ConfirmVerificationAdminCommand
        {
            Jwt = new Jwt([], [new Claim("purpose", "email_verification")])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin's verification status is already confirmed.");
    }
}