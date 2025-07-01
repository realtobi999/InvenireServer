using InvenireServer.Application.Core.Properties.Command.Create;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Properties.Commands;

public class CreatePropertyCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly CreatePropertyCommandHandler _handler;

    public CreatePropertyCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new CreatePropertyCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectPropertyInstance()
    {
        // Prepare
        var organization = new OrganizationFaker().Generate();
        var admin = new AdminFaker(organization).Generate();

        // Assign the admin to the organization.
        organization.Admin = admin;

        var command = new CreatePropertyCommand
        {
            Id = Guid.NewGuid(),
            Jwt = new Jwt([], []),
            OrganizationId = organization.Id,
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.GetAsync(o => o.Id == command.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.CreateAsync(It.IsAny<Property>()));
        _services.Setup(s => s.Organizations.UpdateAsync(organization));

        // Act & Assert.
        var result = await _handler.Handle(command, new CancellationToken());

        // Assert that the property is correctly constructed.
        var property = result.Property;
        property.Id.Should().Be(command.Id.ToString());
        property.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        property.LastUpdatedAt.Should().BeNull();

        // Assert that the property is part of the organization.
        organization.Property.Should().NotBeNull();
        organization.Property!.Id.Should().Be(property.Id);
        property.OrganizationId.Should().Be(organization.Id);
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheAdminIsNotOwner()
    {
        // Prepare
        var organization = new OrganizationFaker().Generate();
        var admin = new AdminFaker(organization).Generate();

        // Assign a another admin to the organization.
        organization.Admin = new AdminFaker().Generate();

        var command = new CreatePropertyCommand
        {
            Id = Guid.NewGuid(),
            Jwt = new Jwt([], []),
            OrganizationId = organization.Id,
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.GetAsync(o => o.Id == command.OrganizationId)).ReturnsAsync(organization);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<Unauthorized401Exception>();
    }
}
