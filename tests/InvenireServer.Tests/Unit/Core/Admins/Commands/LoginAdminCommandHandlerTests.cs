using System.Linq.Expressions;
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
    private readonly JwtManager _jwt;
    private readonly IPasswordHasher<Admin> _hasher;
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly LoginAdminCommandHandler _handler;

    public LoginAdminCommandHandlerTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _hasher = new PasswordHasher<Admin>();
        _jwt = JwtManagerFaker.Initiate();

        _handler = new LoginAdminCommandHandler(_jwt, _hasher, _repositories.Object);
    }

    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new LoginAdminCommand
        {
            EmailAddress = admin.EmailAddress,
            Password = admin.Password,
        };

        // Hash the password before storing it, matching the format used in the database.
        admin.Password = _hasher.HashPassword(admin, admin.Password);
        // Set the admin as verified.
        admin.IsVerified = true;

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(a => a.EmailAddress == command.EmailAddress)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Admins.ExecuteUpdateAsync(It.IsAny<Admin>())).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        admin.LastLoginAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

        var result = await action.Invoke();
        result.Should().NotBeNull();
        result.Token.Should().NotBeNull();
        result.Token.Payload.Should().NotBeEmpty();
        result.Token.Payload.Should().Contain(c => c.Type == "role" && c.Value == Jwt.Roles.ADMIN);
        result.Token.Payload.Should().Contain(c => c.Type == "admin_id" && c.Value == admin.Id.ToString());
        result.Token.Payload.Should().Contain(c => c.Type == "is_verified" && c.Value == bool.TrueString);
        result.TokenString.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenAdminIsNotFound()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new LoginAdminCommand
        {
            EmailAddress = admin.EmailAddress,
            Password = admin.Password,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(a => a.EmailAddress == command.EmailAddress)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenAdminIsNotVerified()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new LoginAdminCommand
        {
            EmailAddress = admin.EmailAddress,
            Password = admin.Password,
        };

        // Set the admin as not verified.
        admin.IsVerified = false;

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(a => a.EmailAddress == command.EmailAddress)).ReturnsAsync(admin);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("Verification is required to proceed with login.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenPasswordsDontMatch()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new LoginAdminCommand
        {
            EmailAddress = admin.EmailAddress,
            Password = admin.Password.Reverse().ToString()!,
        };

        // Hash the password before storing it, matching the format used in the database.
        admin.Password = _hasher.HashPassword(admin, admin.Password);
        // Set the admin as verified.
        admin.IsVerified = true;

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(a => a.EmailAddress == command.EmailAddress)).ReturnsAsync(admin);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("Invalid credentials.");
    }
}
