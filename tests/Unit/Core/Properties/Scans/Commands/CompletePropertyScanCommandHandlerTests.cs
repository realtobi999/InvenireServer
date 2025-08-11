using InvenireServer.Application.Core.Properties.Scans.Commands.Complete;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Properties;
using InvenireServer.Tests.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Properties.Scans.Commands;

public class CompletePropertyScanCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly CompletePropertyScanCommandHandler _handler;

    public CompletePropertyScanCommandHandlerTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _handler = new CompletePropertyScanCommandHandler(_repositories.Object);
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

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Scans.GetInProgressForAsync(property)).ReturnsAsync(scan);
        _repositories.Setup(r => r.Properties.Scans.Update(scan));
        _repositories.Setup(r => r.SaveOrThrowAsync());

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        // Assert that the scan is properly completed.
        scan.Status.Should().Be(PropertyScanStatus.COMPLETED);
        scan.CompletedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheAdminDoesntOwnOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: null);

        var command = new CompletePropertyScanCommand
        {
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin doesn't own a organization.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenThePropertyIsNotCreated()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, property: null);

        var command = new CompletePropertyScanCommand
        {
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The organization doesn't have a property.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenThereIsNoActiveScan()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake(scans: []);
        var organization = OrganizationFaker.Fake(admin: admin, property: property);

        var command = new CompletePropertyScanCommand
        {
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Scans.GetInProgressForAsync(property)).ReturnsAsync((PropertyScan?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The organization doesn't have an active scan.");
    }
}
