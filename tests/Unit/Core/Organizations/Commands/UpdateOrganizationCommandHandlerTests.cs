using InvenireServer.Application.Core.Organizations.Commands.Update;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Organizations.Commands;

public class UpdateOrganizationCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly UpdateOrganizationCommandHandler _handler;

    public UpdateOrganizationCommandHandlerTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _handler = new UpdateOrganizationCommandHandler(_repositories.Object);
    }

    [Fact]
    public async Task Handle_UpdatesOrganizationCorrectly()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin);

        var command = new UpdateOrganizationCommand
        {
            Jwt = new Jwt([], []),
            Name = new Faker().Lorem.Sentence()
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Organizations.Update(It.IsAny<Organization>()));
        _repositories.Setup(r => r.SaveOrThrowAsync());

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().NotThrowAsync();

        // Assert that the organization is correctly updated.
        organization.Name.Should().Be(command.Name);
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenAdminIsNotFound()
    {
        // Prepare.
        var command = new UpdateOrganizationCommand
        {
            Jwt = new Jwt([], []),
            Name = new Faker().Lorem.Sentence()
        };

        _repositories.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenAdminDoesntOwnAnOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: null);

        var command = new UpdateOrganizationCommand
        {
            Jwt = new Jwt([], []),
            Name = new Faker().Lorem.Sentence()
        };

        _repositories.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(s => s.Organizations.GetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin doesn't own a organization.");
    }
}
