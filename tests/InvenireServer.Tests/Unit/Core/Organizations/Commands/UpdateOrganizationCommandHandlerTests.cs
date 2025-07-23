using InvenireServer.Application.Core.Organizations.Commands.Update;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Organizations.Commands;

public class UpdateOrganizationCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly UpdateOrganizationCommandHandler _handler;

    public UpdateOrganizationCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new UpdateOrganizationCommandHandler(_services.Object);
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

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);
        _services.Setup(s => s.Organizations.UpdateAsync(It.IsAny<Organization>()));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().NotThrowAsync();

        // Assert that the organization is correctly updated.
        organization.Name.Should().Be(command.Name);
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

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("You have not created an organization.");
    }
}
