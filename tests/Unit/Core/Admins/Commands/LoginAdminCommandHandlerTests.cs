using InvenireServer.Application.Core.Admins.Commands.Login;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Tests.Fakers.Common;
using InvenireServer.Tests.Fakers.Users;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Tests.Unit.Core.Admins.Commands;

public class LoginAdminCommandHandlerTests
{
    private readonly PasswordHasher<Admin> _hasher;
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly LoginAdminCommandHandler _handler;

    public LoginAdminCommandHandlerTests()
    {
        _hasher = new PasswordHasher<Admin>();
        _repositories = new Mock<IRepositoryManager>();
        _handler = new LoginAdminCommandHandler(JwtManagerFaker.Initiate(), _hasher, _repositories.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectToken()
    {
        // Prepare
        var admin = AdminFaker.Fake();

        var command = new LoginAdminCommand
        {
            EmailAddress = admin.EmailAddress,
            Password = admin.Password
        };

        admin.IsVerified = true;
        admin.Password = _hasher.HashPassword(admin, admin.Password);

        _repositories.Setup(r => r.Admins.GetAsync(a => a.EmailAddress == command.EmailAddress)).ReturnsAsync(admin);
        _repositories.Setup(r => r.SaveOrThrowAsync());

        // Act & Assert
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert that the token has all the necessary claims.
        var jwt = JwtBuilder.Parse(result.Token);
        jwt.Payload.Should().Contain(c => c.Type == "role" && c.Value == Jwt.Roles.ADMIN);
        jwt.Payload.Should().Contain(c => c.Type == "admin_id" && c.Value == admin.Id.ToString());
        jwt.Payload.Should().Contain(c => c.Type == "is_verified" && c.Value == bool.TrueString);

        // Assert that the admin has updated his last login timestamp.
        admin.LastLoginAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenAdminIsNotFound()
    {
        // Prepare
        var admin = AdminFaker.Fake();

        var command = new LoginAdminCommand
        {
            EmailAddress = new string([.. admin.EmailAddress.Reverse()]), // Set invalid email.
            Password = admin.Password
        };

        admin.IsVerified = true;
        admin.Password = _hasher.HashPassword(admin, admin.Password);

        _repositories.Setup(r => r.Admins.GetAsync(e => e.EmailAddress == command.EmailAddress)).ThrowsAsync(new NotFound404Exception());

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenIncorrectPassword()
    {
        // Prepare
        var admin = AdminFaker.Fake();

        var command = new LoginAdminCommand
        {
            EmailAddress = admin.EmailAddress,
            Password = new string([.. admin.Password.Reverse()]) // Set incorrect password
        };

        admin.IsVerified = true;
        admin.Password = _hasher.HashPassword(admin, admin.Password);

        _repositories.Setup(r => r.Admins.GetAsync(e => e.EmailAddress == command.EmailAddress)).ReturnsAsync(admin);

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenAdminIsNotVerified()
    {
        // Prepare
        var admin = AdminFaker.Fake();

        var command = new LoginAdminCommand
        {
            EmailAddress = admin.EmailAddress,
            Password = admin.Password
        };

        admin.IsVerified = false; // Set the admin as unverified.
        admin.Password = _hasher.HashPassword(admin, admin.Password);

        _repositories.Setup(r => r.Admins.GetAsync(e => e.EmailAddress == command.EmailAddress)).ReturnsAsync(admin);

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("Verification is required to proceed with login.");
    }
}