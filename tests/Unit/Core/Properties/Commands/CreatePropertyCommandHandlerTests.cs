using InvenireServer.Application.Core.Properties.Commands.Create;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Properties;
using InvenireServer.Tests.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Properties.Commands;

public class CreatePropertyCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly CreatePropertyCommandHandler _handler;

    public CreatePropertyCommandHandlerTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _handler = new CreatePropertyCommandHandler(_repositories.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectPropertyInstance()
    {
        // Prepare
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin);

        var command = new CreatePropertyCommand
        {
            Id = Guid.NewGuid(),
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync((Property?)null);
        _repositories.Setup(r => r.Properties.Create(It.IsAny<Property>()));
        _repositories.Setup(r => r.Organizations.Update(organization));
        _repositories.Setup(r => r.SaveOrThrowAsync());

        // Act & Assert.
        var result = await _handler.Handle(command, CancellationToken.None);

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
    public async Task Handle_ThrowsExceptionWhenTheAdminIsNotFound()
    {
        // Prepare
        var command = new CreatePropertyCommand
        {
            Id = Guid.NewGuid(),
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin doesn't own a organization.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheAdminDoesntOwnAnOrganization()
    {
        // Prepare
        var admin = AdminFaker.Fake();

        var command = new CreatePropertyCommand
        {
            Id = Guid.NewGuid(),
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin doesn't own a organization.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheOrganizationAlreadyHasAProperty()
    {
        // Prepare
        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, property: property);

        var command = new CreatePropertyCommand
        {
            Id = Guid.NewGuid(),
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Conflict409Exception>().WithMessage("The organization already has a property.");
    }
}