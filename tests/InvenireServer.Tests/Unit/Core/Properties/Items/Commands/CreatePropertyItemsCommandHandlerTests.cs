using System.Linq.Expressions;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Extensions.Properties;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Properties;
using InvenireServer.Tests.Integration.Fakers.Properties.Items;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Properties.Items.Commands;

public class CreatePropertyItemsCommandHandlerTests
{
    private readonly CreatePropertyItemsCommandHandler _handler;
    private readonly Mock<IServiceManager> _services;

    public CreatePropertyItemsCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new CreatePropertyItemsCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_AssignsCorrectItemsToProperty()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, property: property, employees: [employee]);

        // We cant add the items to the employee because that is  the  point  of
        // this test. We must only assign the id of the employee to the item  so
        // it can be passed to the command.
        foreach (var item in items) item.AssignEmployee(employee);

        var command = new CreatePropertyItemsCommand
        {
            Items = [.. items.Select(i => i.ToCreatePropertyItemCommand())],
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
        _services.Setup(s => s.Employees.GetAsync(It.IsAny<Expression<Func<Employee, bool>>>())).ReturnsAsync(employee);
        _services.Setup(s => s.Properties.Items.CreateAsync(It.IsAny<IEnumerable<PropertyItem>>()));
        _services.Setup(s => s.Employees.UpdateAsync(It.IsAny<IEnumerable<Employee>>()));

        // Act & Assert.
        await _handler.Handle(command, CancellationToken.None);

        // Assert that the property has the correctly constructed items.
        for (var i = 0; i < items.Count; i++)
        {
            property.Items[i].Id.Should().Be(items[i].Id);
            property.Items[i].InventoryNumber.Should().Be(items[i].InventoryNumber);
            property.Items[i].RegistrationNumber.Should().Be(items[i].RegistrationNumber);
            property.Items[i].Name.Should().Be(items[i].Name);
            property.Items[i].Price.Should().Be(items[i].Price);
            property.Items[i].SerialNumber.Should().Be(items[i].SerialNumber);
            property.Items[i].DateOfPurchase.Should().Be(items[i].DateOfPurchase);
            property.Items[i].DateOfSale.Should().Be(items[i].DateOfSale);
            property.Items[i].Location.Room.Should().Be(items[i].Location.Room);
            property.Items[i].Location.Building.Should().Be(items[i].Location.Building);
            property.Items[i].Location.AdditionalNote.Should().Be(items[i].Location.AdditionalNote);
            property.Items[i].Description.Should().Be(items[i].Description);
            property.Items[i].DocumentNumber.Should().Be(items[i].DocumentNumber);
            property.Items[i].CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
            property.Items[i].LastUpdatedAt.Should().BeNull();
        }

        // Assert that the employee has the assigned items.
        employee.AssignedItems.Select(i => i.Id).Should().Contain(items.Select(i => i.Id));
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenAdminDoesntOwnAnOrganization()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: null, property: property);

        var command = new CreatePropertyItemsCommand
        {
            Items = [],
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync((Organization?)null);
        _services.Setup(s => s.Properties.GetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync(property);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("You have not created an organization. You must create an organization before modifying your property.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenPropertyIsNotCreated()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, property: null);

        var command = new CreatePropertyItemsCommand
        {
            Items = [],
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("You have not created a property. You must create a property before creating its items.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenEmployeeIsNotInOrganization()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake(items: items);
        var employee = EmployeeFaker.Fake(items: items);
        var organization = OrganizationFaker.Fake(admin: admin, property: property, employees: null);

        var command = new CreatePropertyItemsCommand
        {
            Items = [.. items.Select(i => i.ToCreatePropertyItemCommand())],
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
        _services.Setup(s => s.Employees.GetAsync(It.IsAny<Expression<Func<Employee, bool>>>())).ReturnsAsync(employee);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("Cannot assign property to a employee from a another organization.");
    }
}