using InvenireServer.Application.Core.Admins.Commands.Register;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Tests.Extensions;
using InvenireServer.Tests.Fakers.Common;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Tests.Unit.Core.Admins.Commands;

public class RegisterAdminCommandHandlerTests
{
    private readonly PasswordHasher<Admin> _hasher;
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly RegisterAdminCommandHandler _handler;

    public RegisterAdminCommandHandlerTests()
    {
        _hasher = new PasswordHasher<Admin>();
        _repositories = new Mock<IRepositoryManager>();
        _handler = new RegisterAdminCommandHandler(JwtManagerFaker.Initiate(), _hasher, _repositories.Object);
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

        _repositories.Setup(r => r.Admins.Create(It.IsAny<Admin>()));
        _repositories.Setup(r => r.SaveOrThrowAsync());

        // Act & Assert.
        var result = await _handler.Handle(command, CancellationToken.None);

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
        var jwt = JwtBuilder.Parse(result.Response.Token);
        jwt.Payload.Should().Contain(c => c.Type == "role" && c.Value == Jwt.Roles.ADMIN);
        jwt.Payload.Should().Contain(c => c.Type == "admin_id" && c.Value == command.Id.ToString());
        jwt.Payload.Should().Contain(c => c.Type == "is_verified" && c.Value == bool.FalseString);

    }
}