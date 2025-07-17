using InvenireServer.Application.Core.Properties.Scans.Commands.Create;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Properties;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Properties.Scans.Commands;

public class CreatePropertyScanCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly CreatePropertyScanCommandHandler _handler;

    public CreatePropertyScanCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new CreatePropertyScanCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectInstance()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, property: property);

        var faker = new Faker();
        var command = new CreatePropertyScanCommand
        {
            Id = Guid.NewGuid(),
            Name = faker.Lorem.Sentence(3),
            Description = faker.Lorem.Paragraph(),
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Scans.IndexInProgressAsync(property)).ReturnsAsync([]);
        _services.Setup(s => s.Properties.Scans.CreateAsync(It.IsAny<PropertyScan>()));

        // Act & Assert.
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert that the suggestion is correctly constructed.
        result.Scan.Id.Should().Be(command.Id.ToString());
        result.Scan.Name.Should().Be(command.Name);
        result.Scan.Description.Should().Be(command.Description);
        result.Scan.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        result.Scan.CompletedAt.Should().BeNull();
        result.Scan.LastUpdatedAt.Should().BeNull();
        result.Scan.PropertyId.Should().Be(property.Id);
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenAdminDoesntOwnOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: null, property: property);

        var faker = new Faker();
        var command = new CreatePropertyScanCommand
        {
            Id = Guid.NewGuid(),
            Name = faker.Lorem.Sentence(3),
            Description = faker.Lorem.Paragraph(),
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("You do not own a organization.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenPropertyIsNotCreated()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, property: null);

        var faker = new Faker();
        var command = new CreatePropertyScanCommand
        {
            Id = Guid.NewGuid(),
            Name = faker.Lorem.Sentence(3),
            Description = faker.Lorem.Paragraph(),
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("You have not created a property.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenAnotherActiveScanIsPresent()
    {
        // Prepare.
        var scan = PropertyScanFaker.Fake();
        scan.Status = PropertyScanStatus.IN_PROGRESS;

        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake(scans: [scan]);
        var organization = OrganizationFaker.Fake(admin: admin, property: property);

        var faker = new Faker();
        var command = new CreatePropertyScanCommand
        {
            Id = Guid.NewGuid(),
            Name = faker.Lorem.Sentence(3),
            Description = faker.Lorem.Paragraph(),
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Scans.IndexInProgressAsync(property)).ReturnsAsync([scan]);


        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Conflict409Exception>().WithMessage("An active property scan is already created.");
    }
}
