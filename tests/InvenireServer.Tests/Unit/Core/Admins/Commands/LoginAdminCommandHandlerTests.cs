using InvenireServer.Application.Core.Admins.Commands.Login;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Tests.Integration.Fakers.Common;
using InvenireServer.Tests.Integration.Fakers.Users;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Tests.Unit.Core.Admins.Commands;

public class LoginAdminCommandHandlerTests
{
    private readonly LoginAdminCommandHandler _handler;
    private readonly PasswordHasher<Admin> _hasher;
    private readonly IJwtManager _jwt;
    private readonly Mock<IServiceManager> _services;

    public LoginAdminCommandHandlerTests()
    {
        _jwt = new JwtManagerFaker().Initiate();
        _hasher = new PasswordHasher<Admin>();
        _services = new Mock<IServiceManager>();
        _handler = new LoginAdminCommandHandler(_services.Object, _hasher, _jwt);
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

        _services.Setup(s => s.Admins.GetAsync(e => e.EmailAddress == command.EmailAddress)).ReturnsAsync(admin);

        // Act & Assert
        var result = await _handler.Handle(command, new CancellationToken());

        // Assert that the token has all the necessary claims.
        var jwt = JwtBuilder.Parse(result.Token);
        jwt.Payload.Should().Contain(c => c.Type == "role" && c.Value == Jwt.Roles.ADMIN);
        jwt.Payload.Should().Contain(c => c.Type == "admin_id" && c.Value == admin.Id.ToString());
        jwt.Payload.Should().Contain(c => c.Type == "is_verified" && c.Value == bool.TrueString);

        // Assert that the admin has updated his last login timestamp.
        admin.LastLoginAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenAdminIsNotFound()
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

        _services.Setup(s => s.Admins.GetAsync(e => e.EmailAddress == command.EmailAddress)).ThrowsAsync(new NotFound404Exception());

        // Act & Assert
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenIncorrectPassword()
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

        _services.Setup(s => s.Admins.GetAsync(e => e.EmailAddress == command.EmailAddress)).ReturnsAsync(admin);

        // Act & Assert
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenAdminIsNotVerified()
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

        _services.Setup(s => s.Admins.GetAsync(e => e.EmailAddress == command.EmailAddress)).ReturnsAsync(admin);

        // Act & Assert
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("Verification required to proceed.");
    }
}