using InvenireServer.Application.Core.Properties.Items.Commands.Scan;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Properties;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Properties.Items.Commands;

public class ScanPropertyItemCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly ScanPropertyItemCommandHandler _handler;

    public ScanPropertyItemCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new ScanPropertyItemCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_ScansItemCorrectly()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var item = PropertyItemFaker.Fake();
        var scan = PropertyScanFaker.Fake();
        var employee = EmployeeFaker.Fake(items: [item]);
        var property = PropertyFaker.Fake(items: [item], scans: [scan]);
        var organization = OrganizationFaker.Fake(admin: admin, property: property, employees: [employee]);

        var command = new ScanPropertyItemCommand
        {
            Jwt = new Jwt([], [new("role", Jwt.Roles.EMPLOYEE)]),
            ItemId = item.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Properties.Items.GetAsync(i => i.Id == command.ItemId)).ReturnsAsync(item);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == employee.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Scans.TryGetAsync(s => !s.CompletedAt.HasValue)).ReturnsAsync(scan);

        // Act & Assert.
        await _handler.Handle(command, CancellationToken.None);

        // Assert that the scan's items contains the item.
        scan.ScannedItems.Should().Contain(i => i.Id == item.Id);
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenAdminDoesntOwnOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var item = PropertyItemFaker.Fake();
        var scan = PropertyScanFaker.Fake();
        var employee = EmployeeFaker.Fake(items: [item]);
        var property = PropertyFaker.Fake(items: [item], scans: [scan]);
        var organization = OrganizationFaker.Fake(admin: null, property: property, employees: [employee]);

        var command = new ScanPropertyItemCommand
        {
            Jwt = new Jwt([], [new("role", Jwt.Roles.EMPLOYEE)]),
            ItemId = item.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Properties.Items.GetAsync(i => i.Id == command.ItemId)).ReturnsAsync(item);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == employee.OrganizationId)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("You are not part of an organization.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenPropertyIsNotCreated()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var item = PropertyItemFaker.Fake();
        var scan = PropertyScanFaker.Fake();
        var employee = EmployeeFaker.Fake(items: [item]);
        var organization = OrganizationFaker.Fake(admin: admin, property: null, employees: [employee]);

        var command = new ScanPropertyItemCommand
        {
            Jwt = new Jwt([], [new("role", Jwt.Roles.EMPLOYEE)]),
            ItemId = item.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Properties.Items.GetAsync(i => i.Id == command.ItemId)).ReturnsAsync(item);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == employee.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("Organization you are part of doesn't have a property.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenNoActiveScanIsPresent()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var item = PropertyItemFaker.Fake();
        var employee = EmployeeFaker.Fake(items: [item]);
        var property = PropertyFaker.Fake(items: [item], scans: []);
        var organization = OrganizationFaker.Fake(admin: admin, property: property, employees: [employee]);

        var command = new ScanPropertyItemCommand
        {
            Jwt = new Jwt([], [new("role", Jwt.Roles.EMPLOYEE)]),
            ItemId = item.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Properties.Items.GetAsync(i => i.Id == command.ItemId)).ReturnsAsync(item);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == employee.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Scans.TryGetAsync(s => !s.CompletedAt.HasValue)).ReturnsAsync((PropertyScan?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("There is currently no active scans.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheItemIsNotAssignedToTheEmployee()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var item = PropertyItemFaker.Fake();
        var scan = PropertyScanFaker.Fake();
        var employee = EmployeeFaker.Fake(items: []);
        var property = PropertyFaker.Fake(items: [item], scans: [scan]);
        var organization = OrganizationFaker.Fake(admin: admin, property: property, employees: [employee]);

        var command = new ScanPropertyItemCommand
        {
            Jwt = new Jwt([], [new("role", Jwt.Roles.EMPLOYEE)]),
            ItemId = item.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Properties.Items.GetAsync(i => i.Id == command.ItemId)).ReturnsAsync(item);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == employee.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Scans.TryGetAsync(s => !s.CompletedAt.HasValue)).ReturnsAsync(scan);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Unauthorized401Exception>();
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheItemIsNotPartOfTheProperty()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var item = PropertyItemFaker.Fake();
        var scan = PropertyScanFaker.Fake();
        var employee = EmployeeFaker.Fake(items: [item]);
        var property = PropertyFaker.Fake(items: [], scans: [scan]);
        var organization = OrganizationFaker.Fake(admin: admin, property: property, employees: [employee]);

        var command = new ScanPropertyItemCommand
        {
            Jwt = new Jwt([], [new("role", Jwt.Roles.EMPLOYEE)]),
            ItemId = item.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Properties.Items.GetAsync(i => i.Id == command.ItemId)).ReturnsAsync(item);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == employee.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Scans.TryGetAsync(s => !s.CompletedAt.HasValue)).ReturnsAsync(scan);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The item isn't a part of the property.");
    }
}
