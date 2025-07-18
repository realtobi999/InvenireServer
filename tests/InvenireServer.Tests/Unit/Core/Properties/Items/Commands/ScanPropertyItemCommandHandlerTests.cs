using InvenireServer.Application.Core.Properties.Items.Commands.Scan;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Properties;
using InvenireServer.Tests.Integration.Fakers.Properties.Items;
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
        var scan = PropertyScanFaker.Fake();
        scan.Status = PropertyScanStatus.IN_PROGRESS;

        var item = PropertyItemFaker.Fake();
        var employee = EmployeeFaker.Fake(items: [item]);
        var property = PropertyFaker.Fake(items: [item], scans: [scan]);
        var organization = OrganizationFaker.Fake(property: property, employees: [employee]);

        var command = new ScanPropertyItemCommand
        {
            Jwt = new Jwt([], [new("role", Jwt.Roles.EMPLOYEE)]),
            ItemId = item.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Properties.Items.GetAsync(i => i.Id == command.ItemId)).ReturnsAsync(item);
        _services.Setup(s => s.Organizations.TryGetForAsync(employee)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Scans.TryGetInProgressForAsync(property)).ReturnsAsync(scan);
        _services.Setup(s => s.Properties.Scans.UpdateAsync(It.IsAny<PropertyScan>()));

        // Act & Assert.
        await _handler.Handle(command, CancellationToken.None);

        // Assert that the scan's items contains the item.
        scan.ScannedItems.Should().Contain(i => i.Id == item.Id);
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenEmployeeIsNotInOrganization()
    {
        // Prepare.
        var item = PropertyItemFaker.Fake();
        var employee = EmployeeFaker.Fake(items: [item]);
        var organization = OrganizationFaker.Fake(employees: []);

        var command = new ScanPropertyItemCommand
        {
            Jwt = new Jwt([], [new("role", Jwt.Roles.EMPLOYEE)]),
            ItemId = item.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Properties.Items.GetAsync(i => i.Id == command.ItemId)).ReturnsAsync(item);
        _services.Setup(s => s.Organizations.TryGetForAsync(employee)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("You are not part of an organization.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenPropertyIsNotCreated()
    {
        // Prepare.
        var item = PropertyItemFaker.Fake();
        var employee = EmployeeFaker.Fake(items: [item]);
        var organization = OrganizationFaker.Fake(property: null, employees: [employee]);

        var command = new ScanPropertyItemCommand
        {
            Jwt = new Jwt([], [new("role", Jwt.Roles.EMPLOYEE)]),
            ItemId = item.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Properties.Items.GetAsync(i => i.Id == command.ItemId)).ReturnsAsync(item);
        _services.Setup(s => s.Organizations.TryGetForAsync(employee)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("Organization you are part of doesn't have a property.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenNoActiveScanIsPresent()
    {
        // Prepare.
        var item = PropertyItemFaker.Fake();
        var employee = EmployeeFaker.Fake(items: [item]);
        var property = PropertyFaker.Fake(items: [item], scans: []);
        var organization = OrganizationFaker.Fake(property: property, employees: [employee]);

        var command = new ScanPropertyItemCommand
        {
            Jwt = new Jwt([], [new("role", Jwt.Roles.EMPLOYEE)]),
            ItemId = item.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Properties.Items.GetAsync(i => i.Id == command.ItemId)).ReturnsAsync(item);
        _services.Setup(s => s.Organizations.TryGetForAsync(employee)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Scans.TryGetInProgressForAsync(property)).ReturnsAsync((PropertyScan?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("There are currently no active scans.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenItemIsNotAssignedToEmployee()
    {
        // Prepare.
        var scan = PropertyScanFaker.Fake();
        scan.Status = PropertyScanStatus.IN_PROGRESS;

        var item = PropertyItemFaker.Fake();
        var employee = EmployeeFaker.Fake(items: []);
        var property = PropertyFaker.Fake(items: [item], scans: [scan]);
        var organization = OrganizationFaker.Fake(property: property, employees: [employee]);

        var command = new ScanPropertyItemCommand
        {
            Jwt = new Jwt([], [new("role", Jwt.Roles.EMPLOYEE)]),
            ItemId = item.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Properties.Items.GetAsync(i => i.Id == command.ItemId)).ReturnsAsync(item);
        _services.Setup(s => s.Organizations.TryGetForAsync(employee)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Scans.TryGetInProgressForAsync(property)).ReturnsAsync(scan);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Unauthorized401Exception>();
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenItemIsNotPartOfProperty()
    {
        // Prepare.
        var scan = PropertyScanFaker.Fake();
        scan.Status = PropertyScanStatus.IN_PROGRESS;

        var item = PropertyItemFaker.Fake();
        var employee = EmployeeFaker.Fake(items: [item]);
        var property = PropertyFaker.Fake(items: [], scans: [scan]);
        var organization = OrganizationFaker.Fake(property: property, employees: [employee]);

        var command = new ScanPropertyItemCommand
        {
            Jwt = new Jwt([], [new("role", Jwt.Roles.EMPLOYEE)]),
            ItemId = item.Id
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Properties.Items.GetAsync(i => i.Id == command.ItemId)).ReturnsAsync(item);
        _services.Setup(s => s.Organizations.TryGetForAsync(employee)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Scans.TryGetInProgressForAsync(property)).ReturnsAsync(scan);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The item isn't a part of the property.");
    }
}
