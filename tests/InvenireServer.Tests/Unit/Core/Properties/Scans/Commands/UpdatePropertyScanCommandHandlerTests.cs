using InvenireServer.Application.Core.Properties.Scans.Commands.Update;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Properties;
using InvenireServer.Tests.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Properties.Scans.Commands;

public class UpdatePropertyScanCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly UpdatePropertyScanCommandHandler _handler;

    public UpdatePropertyScanCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new UpdatePropertyScanCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_UpdatesScanCorrectly()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var scan = PropertyScanFaker.Fake();
        var property = PropertyFaker.Fake(scans: [scan]);
        var organization = OrganizationFaker.Fake(admin: admin, property: property);

        var faker = new Faker();
        var command = new UpdatePropertyScanCommand
        {
            Name = faker.Lorem.Sentence(),
            Description = faker.Lorem.Sentences(),
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Scans.TryGetInProgressForAsync(property)).ReturnsAsync(scan);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().NotThrowAsync();

        // Assert that the scan is correctly updated.
        scan.Name.Should().Be(command.Name);
        scan.Description.Should().Be(command.Description);
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenAdminDoesntOwnAnOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var scan = PropertyScanFaker.Fake();
        var property = PropertyFaker.Fake(scans: [scan]);
        var organization = OrganizationFaker.Fake(admin: null, property: property);

        var faker = new Faker();
        var command = new UpdatePropertyScanCommand
        {
            Name = faker.Lorem.Sentence(),
            Description = faker.Lorem.Sentences(),
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("You do not own an organization.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenPropertyIsNotCreated()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var scan = PropertyScanFaker.Fake();
        var property = PropertyFaker.Fake(scans: [scan]);
        var organization = OrganizationFaker.Fake(admin: admin, property: null);

        var faker = new Faker();
        var command = new UpdatePropertyScanCommand
        {
            Name = faker.Lorem.Sentence(),
            Description = faker.Lorem.Sentences(),
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("You have not created a property.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenActiveScanIsNotPresent()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var scan = PropertyScanFaker.Fake();
        var property = PropertyFaker.Fake(scans: [scan]);
        var organization = OrganizationFaker.Fake(admin: admin, property: property);

        scan.Status = PropertyScanStatus.COMPLETED;

        var faker = new Faker();
        var command = new UpdatePropertyScanCommand
        {
            Name = faker.Lorem.Sentence(),
            Description = faker.Lorem.Sentences(),
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Scans.TryGetInProgressForAsync(property)).ReturnsAsync((PropertyScan?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("There are currently no active scans.");
    }
}
