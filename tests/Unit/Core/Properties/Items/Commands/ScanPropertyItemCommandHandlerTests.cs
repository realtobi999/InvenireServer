using InvenireServer.Application.Core.Properties.Items.Commands.Scan;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Properties;
using InvenireServer.Tests.Fakers.Properties.Items;
using InvenireServer.Tests.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Properties.Items.Commands;

public class ScanPropertyItemCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly ScanPropertyItemCommandHandler _handler;

    public ScanPropertyItemCommandHandlerTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _handler = new ScanPropertyItemCommandHandler(_repositories.Object);
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

        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Properties.Items.GetAsync(i => i.Id == command.ItemId)).ReturnsAsync(item);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Scans.GetInProgressForAsync(property)).ReturnsAsync(scan);
        _repositories.Setup(r => r.Properties.Scans.Update(It.IsAny<PropertyScan>()));
        _repositories.Setup(r => r.SaveOrThrowAsync());

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        // Assert that the scan's items contains the item.
        scan.ScannedItems.Should().Contain(i => i.Id == item.Id);
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheEmployeeIsNotInOrganization()
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

        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Properties.Items.GetAsync(i => i.Id == command.ItemId)).ReturnsAsync(item);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The employee isn't part of any organization.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenThePropertyIsNotCreated()
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

        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Properties.Items.GetAsync(i => i.Id == command.ItemId)).ReturnsAsync(item);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The organization doesn't have a property.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenThereIsNoActiveScans()
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

        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Properties.Items.GetAsync(i => i.Id == command.ItemId)).ReturnsAsync(item);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Scans.GetInProgressForAsync(property)).ReturnsAsync((PropertyScan?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The organization doesn't have an active scan.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheItemIsNotAssignedToEmployee()
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

        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Properties.Items.GetAsync(i => i.Id == command.ItemId)).ReturnsAsync(item);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Scans.GetInProgressForAsync(property)).ReturnsAsync(scan);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("The item doesn't belong to the item.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheItemIsNotPartOfProperty()
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

        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Properties.Items.GetAsync(i => i.Id == command.ItemId)).ReturnsAsync(item);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Scans.GetInProgressForAsync(property)).ReturnsAsync(scan);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The item isn't part of the property.");
    }
}
