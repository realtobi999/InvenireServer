using InvenireServer.Application.Core.Properties.Scans.Commands.Complete;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Properties;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Properties.Scans.Commands;

public class CompletePropertyScanCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly CompletePropertyScanCommandHandler _handler;

    public CompletePropertyScanCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new CompletePropertyScanCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_CompletesScanCorrectly()
    {
        // Prepare.
        var scan = PropertyScanFaker.Fake();
        scan.Status = PropertyScanStatus.IN_PROGRESS;

        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake(scans: [scan]);
        var organization = OrganizationFaker.Fake(admin: admin, property: property);

        var command = new CompletePropertyScanCommand
        {
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Scans.TryGetInProgressForAsync(property)).ReturnsAsync(scan);
        _services.Setup(s => s.Properties.Scans.UpdateAsync(scan));

        // Act & Assert.
        await _handler.Handle(command, CancellationToken.None);

        // Assert that the scan is properly completed.
        scan.Status.Should().Be(PropertyScanStatus.COMPLETED);
        scan.CompletedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenAdminDoesntOwnOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: null);

        var command = new CompletePropertyScanCommand
        {
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
        var organization = OrganizationFaker.Fake(admin: admin, property: null);

        var command = new CompletePropertyScanCommand
        {
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
    public async Task Handle_ThrowsExceptionWhenNoActiveScanIsPresent()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake(scans: []);
        var organization = OrganizationFaker.Fake(admin: admin, property: property);

        var command = new CompletePropertyScanCommand
        {
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
