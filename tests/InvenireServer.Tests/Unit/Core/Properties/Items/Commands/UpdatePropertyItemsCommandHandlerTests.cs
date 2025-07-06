using System.Linq.Expressions;
using InvenireServer.Application.Core.Properties.Items.Commands.Update;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Properties;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Properties.Items.Commands;

public class UpdatePropertyItemsCommandHandlerTests
{
    private readonly UpdatePropertyItemsCommandHandler _handler;
    private readonly Mock<IServiceManager> _services;

    public UpdatePropertyItemsCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new UpdatePropertyItemsCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_UpdatesItemsCorrectly()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var property = new PropertyFaker(organization).Generate();
        var employee1 = new EmployeeFaker(organization).Generate();
        var employee2 = new EmployeeFaker(organization).Generate();
        var admin = new AdminFaker(organization).Generate();

        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++)
        {
            var item = new PropertyItemFaker(property, employee1).Generate();
            items.Add(item);
            employee1.AssignedItems.Add(item);
        }

        var command = new UpdatePropertyItemsCommand
        {
            Items =
            [
                .. items.Select(i => new UpdatePropertyItemCommand
                {
                    Id = i.Id,
                    InventoryNumber = Guid.NewGuid().ToString(),
                    RegistrationNumber = Guid.NewGuid().ToString(),
                    Name = "TEST",
                    Price = 999,
                    SerialNumber = Guid.NewGuid().ToString(),
                    DateOfPurchase = DateTimeOffset.UtcNow.AddYears(-5),
                    DateOfSale = DateTimeOffset.Now.AddYears(-3),
                    Description = "TEST",
                    DocumentNumber = Guid.NewGuid().ToString(),
                    EmployeeId = employee2.Id
                })
            ],
            Jwt = new Jwt([], []),
            PropertyId = property.Id,
            OrganizationId = organization.Id
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Properties.GetAsync(p => p.Id == command.PropertyId)).ReturnsAsync(property);
        _services.Setup(s => s.Organizations.GetAsync(p => p.Id == command.OrganizationId)).ReturnsAsync(organization);

        var itemQueue = new Queue<PropertyItem>(items);
        _services.Setup(s => s.Properties.Items.GetAsync(It.IsAny<Expression<Func<PropertyItem, bool>>>()))
            .ReturnsAsync(() => itemQueue.Dequeue());

        var employeeQueue = new Queue<Employee>([employee1, employee2]);
        _services.Setup(s => s.Employees.GetAsync(It.IsAny<Expression<Func<Employee, bool>>>()))
            .ReturnsAsync(() => employeeQueue.Dequeue());

        _services.Setup(s => s.Employees.UpdateAsync(It.IsAny<IEnumerable<Employee>>()));
        _services.Setup(s => s.Properties.Items.UpdateAsync(It.IsAny<IEnumerable<PropertyItem>>())).Callback<IEnumerable<PropertyItem>>(arg => items = [.. arg]);

        // Act & Assert.
        await _handler.Handle(command, new CancellationToken());

        // Assert that the property has the correctly updated items.
        for (var i = 0; i < items.Count; i++)
        {
            items[i].Id.Should().Be(command.Items[i].Id);
            items[i].InventoryNumber.Should().Be(command.Items[i].InventoryNumber);
            items[i].RegistrationNumber.Should().Be(command.Items[i].RegistrationNumber);
            items[i].Name.Should().Be(command.Items[i].Name);
            items[i].Price.Should().Be(command.Items[i].Price);
            items[i].SerialNumber.Should().Be(command.Items[i].SerialNumber);
            items[i].DateOfPurchase.Should().Be(command.Items[i].DateOfPurchase);
            items[i].DateOfSale.Should().Be(command.Items[i].DateOfSale);
            items[i].Description.Should().Be(command.Items[i].Description);
            items[i].DocumentNumber.Should().Be(command.Items[i].DocumentNumber);
        }

        // Assert that the first employee doesn't have the assigned items.
        employee1.AssignedItems.Select(i => i.Id).Should().NotContain(items.Select(i => i.Id));
        // Assert that the second employee has the assigned items.
        employee2.AssignedItems.Select(i => i.Id).Should().Contain(items.Select(i => i.Id));
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenAdminIsNotTheOwner()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var property = new PropertyFaker(organization).Generate();
        var admin = new AdminFaker().Generate(); // Organization isn't assigned.

        var command = new UpdatePropertyItemsCommand
        {
            Items = [],
            Jwt = new Jwt([], []),
            PropertyId = property.Id,
            OrganizationId = organization.Id
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Properties.GetAsync(p => p.Id == command.PropertyId)).ReturnsAsync(property);
        _services.Setup(s => s.Organizations.GetAsync(p => p.Id == command.OrganizationId)).ReturnsAsync(organization);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<Unauthorized401Exception>();
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenPropertyIsNotOfOrganization()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var property = new PropertyFaker().Generate(); // Organization isn't assigned.
        var admin = new AdminFaker(organization).Generate();

        var command = new UpdatePropertyItemsCommand
        {
            Items = [],
            Jwt = new Jwt([], []),
            PropertyId = property.Id,
            OrganizationId = organization.Id
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Properties.GetAsync(p => p.Id == command.PropertyId)).ReturnsAsync(property);
        _services.Setup(s => s.Organizations.GetAsync(p => p.Id == command.OrganizationId)).ReturnsAsync(organization);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("This property doesnt belong to your organization.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenItemIsNotOfTheProperty()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var property = new PropertyFaker(organization).Generate();
        var employee = new EmployeeFaker(organization).Generate();
        var admin = new AdminFaker(organization).Generate();

        var items = new List<PropertyItem> { new PropertyItemFaker().Generate() }; // Property isn't assigned.

        var command = new UpdatePropertyItemsCommand
        {
            Items =
            [
                .. items.Select(i => new UpdatePropertyItemCommand
                {
                    Id = i.Id,
                    InventoryNumber = Guid.NewGuid().ToString(),
                    RegistrationNumber = Guid.NewGuid().ToString(),
                    Name = "TEST",
                    Price = 999,
                    SerialNumber = Guid.NewGuid().ToString(),
                    DateOfPurchase = DateTimeOffset.UtcNow.AddYears(-5),
                    DateOfSale = DateTimeOffset.Now.AddYears(-3),
                    Description = "TEST",
                    DocumentNumber = Guid.NewGuid().ToString(),
                    EmployeeId = employee.Id
                })
            ],
            Jwt = new Jwt([], []),
            PropertyId = property.Id,
            OrganizationId = organization.Id
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Properties.GetAsync(p => p.Id == command.PropertyId)).ReturnsAsync(property);
        _services.Setup(s => s.Organizations.GetAsync(p => p.Id == command.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.Items.GetAsync(It.IsAny<Expression<Func<PropertyItem, bool>>>())).ReturnsAsync(items.First());
        _services.Setup(s => s.Employees.GetAsync(It.IsAny<Expression<Func<Employee, bool>>>())).ReturnsAsync(employee);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("Cannot update a item from a property you do not own.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenEmployeeIsNotInOrganization()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var property = new PropertyFaker(organization).Generate();
        var employee1 = new EmployeeFaker(organization).Generate();
        var employee2 = new EmployeeFaker().Generate(); // Organization isn't assigned
        var admin = new AdminFaker(organization).Generate();

        var items = new List<PropertyItem> { new PropertyItemFaker(property, employee1).Generate() };

        var command = new UpdatePropertyItemsCommand
        {
            Items =
            [
                .. items.Select(i => new UpdatePropertyItemCommand
                {
                    Id = i.Id,
                    InventoryNumber = Guid.NewGuid().ToString(),
                    RegistrationNumber = Guid.NewGuid().ToString(),
                    Name = "TEST",
                    Price = 999,
                    SerialNumber = Guid.NewGuid().ToString(),
                    DateOfPurchase = DateTimeOffset.UtcNow.AddYears(-5),
                    DateOfSale = DateTimeOffset.Now.AddYears(-3),
                    Description = "TEST",
                    DocumentNumber = Guid.NewGuid().ToString(),
                    EmployeeId = employee2.Id
                })
            ],
            Jwt = new Jwt([], []),
            PropertyId = property.Id,
            OrganizationId = organization.Id
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Properties.GetAsync(p => p.Id == command.PropertyId)).ReturnsAsync(property);
        _services.Setup(s => s.Organizations.GetAsync(p => p.Id == command.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.Items.GetAsync(It.IsAny<Expression<Func<PropertyItem, bool>>>())).ReturnsAsync(items.First());

        var employeeQueue = new Queue<Employee>([employee1, employee2]);
        _services.Setup(s => s.Employees.GetAsync(It.IsAny<Expression<Func<Employee, bool>>>()))
            .ReturnsAsync(() => employeeQueue.Dequeue());

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("Cannot assign property item to an employee from a another organization.");
    }
}