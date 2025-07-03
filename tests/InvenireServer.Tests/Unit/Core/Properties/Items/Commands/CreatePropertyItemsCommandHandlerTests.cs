using System.Linq.Expressions;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Extensions.Properties;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Properties;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Properties.Items.Commands;

public class CreatePropertyItemsCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly CreatePropertyItemsCommandHandler _handler;

    public CreatePropertyItemsCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new CreatePropertyItemsCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_AssignsCorrectItemsToProperty()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var property = new PropertyFaker(organization).Generate();
        var employee = new EmployeeFaker(organization).Generate();
        var admin = new AdminFaker(organization).Generate();

        var items = new List<PropertyItem>();
        for (int _ = 0; _ < 5; _++) items.Add(new PropertyItemFaker(property, employee).Generate());

        var command = new CreatePropertyItemsCommand
        {
            Items = [.. items.Select(i => i.ToCreatePropertyItemCommand())],
            Jwt = new Jwt([], []),
            PropertyId = property.Id,
            OrganizationId = organization.Id
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Properties.GetAsync(p => p.Id == command.PropertyId)).ReturnsAsync(property);
        _services.Setup(s => s.Organizations.GetAsync(p => p.Id == command.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Employees.GetAsync(It.IsAny<Expression<Func<Employee, bool>>>())).ReturnsAsync(employee);
        _services.Setup(s => s.Properties.Items.CreateAsync(It.IsAny<IEnumerable<PropertyItem>>()));
        _services.Setup(s => s.Employees.UpdateAsync(It.IsAny<IEnumerable<Employee>>()));

        // Act & Assert.
        await _handler.Handle(command, new CancellationToken());

        // Assert that the property has the correctly constructed items.
        for (int i = 0; i < items.Count; i++)
        {
            property.Items[i].Id.Should().Be(items[i].Id);
            property.Items[i].InventoryNumber.Should().Be(items[i].InventoryNumber);
            property.Items[i].RegistrationNumber.Should().Be(items[i].RegistrationNumber);
            property.Items[i].Name.Should().Be(items[i].Name);
            property.Items[i].Price.Should().Be(items[i].Price);
            property.Items[i].SerialNumber.Should().Be(items[i].SerialNumber);
            property.Items[i].DateOfPurchase.Should().Be(items[i].DateOfPurchase);
            property.Items[i].DateOfSale.Should().Be(items[i].DateOfSale);
            property.Items[i].Description.Should().Be(items[i].Description);
            property.Items[i].DocumentNumber.Should().Be(items[i].DocumentNumber);
            property.Items[i].CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
            property.Items[i].LastUpdatedAt.Should().BeNull();
        }
        // Assert that the employee has the assigned items.
        employee.AssignedItems.Select(i => i.Id).Should().Contain(items.Select(i => i.Id));
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenAdminIsNotTheOwner()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var property = new PropertyFaker(organization).Generate();
        var admin = new AdminFaker().Generate(); // Organization isn't assigned.

        var command = new CreatePropertyItemsCommand
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

        var command = new CreatePropertyItemsCommand
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
    public async Task Handle_ThrowsExceptionWhenEmployeeIsNotInOrganization()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var property = new PropertyFaker(organization).Generate();
        var employee = new EmployeeFaker().Generate();  // Organization isn't assigned.
        var admin = new AdminFaker(organization).Generate();

        var items = new List<PropertyItem>();
        for (int _ = 0; _ < 5; _++) items.Add(new PropertyItemFaker(property, employee).Generate());

        var command = new CreatePropertyItemsCommand
        {
            Items = [.. items.Select(i => i.ToCreatePropertyItemCommand())],
            Jwt = new Jwt([], []),
            PropertyId = property.Id,
            OrganizationId = organization.Id
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Properties.GetAsync(p => p.Id == command.PropertyId)).ReturnsAsync(property);
        _services.Setup(s => s.Organizations.GetAsync(p => p.Id == command.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Employees.GetAsync(It.IsAny<Expression<Func<Employee, bool>>>())).ReturnsAsync(employee);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("Cannot assign property to a employee from a another organization.");
    }
}
