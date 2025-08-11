using System.Security.Claims;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Core.Admins.Commands.Verification.Confirm;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Tests.Unit.Core.Admins.Commands;

public class ConfirmVerificationAdminCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly ConfirmVerificationAdminCommandHandler _handler;

    public ConfirmVerificationAdminCommandHandlerTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _handler = new ConfirmVerificationAdminCommandHandler(_repositories.Object);
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

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Admins.Update(admin));
        _repositories.Setup(r => r.SaveOrThrowAsync());

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

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin's verification status is already confirmed.");
    }
}