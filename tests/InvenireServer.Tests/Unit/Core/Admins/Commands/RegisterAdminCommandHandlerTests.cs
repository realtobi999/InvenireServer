using InvenireServer.Application.Core.Admins.Commands.Register;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Tests.Integration.Extensions;
using InvenireServer.Tests.Integration.Fakers.Common;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Tests.Unit.Core.Admins.Commands;

public class RegisterAdminCommandHandlerTests
{
    private readonly RegisterAdminCommandHandler _handler;
    private readonly PasswordHasher<Admin> _hasher;
    private readonly IJwtManager _jwt;
    private readonly Mock<IServiceManager> _services;

    public RegisterAdminCommandHandlerTests()
    {
        _jwt = new JwtManagerFaker().Initiate();
        _hasher = new PasswordHasher<Admin>();
        _services = new Mock<IServiceManager>();
        _handler = new RegisterAdminCommandHandler(_services.Object, _hasher, _jwt);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectTokenAndAdminInstance()
    {
        // Prepare.
        var faker = new Faker();
        var password = faker.Internet.SecurePassword();
        var command = new RegisterAdminCommand
        {
            Id = Guid.NewGuid(),
            Name = faker.Internet.UserName(),
            EmailAddress = faker.Internet.Email(),
            Password = password,
            PasswordConfirm = password
        };

        _services.Setup(s => s.Admins.CreateAsync(It.IsAny<Admin>()));

        // Act & Assert.
        var result = await _handler.Handle(command, new CancellationToken());

        // Assert that the admin is correctly constructed.
        var admin = result.Admin;
        admin.Id.Should().Be(command.Id.ToString());
        admin.Name.Should().Be(command.Name);
        admin.EmailAddress.Should().Be(command.EmailAddress);
        admin.Password.Should().NotBe(password);
        admin.IsVerified.Should().BeFalse();
        admin.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        admin.LastLoginAt.Should().BeNull();
        admin.LastUpdatedAt.Should().BeNull();

        // Assert that the token has all the necessary claims.
        var jwt = JwtBuilder.Parse(result.Token);
        jwt.Payload.Should().Contain(c => c.Type == "role" && c.Value == Jwt.Roles.ADMIN);
        jwt.Payload.Should().Contain(c => c.Type == "admin_id" && c.Value == command.Id.ToString());
        jwt.Payload.Should().Contain(c => c.Type == "is_verified" && c.Value == bool.FalseString);

    }
}