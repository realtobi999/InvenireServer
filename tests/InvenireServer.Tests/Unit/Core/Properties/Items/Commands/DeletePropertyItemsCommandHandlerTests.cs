using System.Linq.Expressions;
using InvenireServer.Application.Core.Properties.Items.Commands.Delete;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Properties;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Properties.Items.Commands;

public class DeletePropertyItemsCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly DeletePropertyItemsCommandHandler _handler;

    public DeletePropertyItemsCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new DeletePropertyItemsCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_DeletesItems()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake(items);
        var employee = EmployeeFaker.Fake(items);
        var organization = OrganizationFaker.Fake(admin: admin, property: property, employees: [employee]);

        var command = new DeletePropertyItemsCommand
        {
            Ids = [.. items.Select(i => i.Id)],
            Jwt = new Jwt([], []),
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync(property);

        var itemQueue = new Queue<PropertyItem>(items);
        _services.Setup(s => s.Properties.Items.GetAsync(It.IsAny<Expression<Func<PropertyItem, bool>>>())).ReturnsAsync(() => itemQueue.Dequeue());

        _services.Setup(s => s.Properties.Items.DeleteAsync(items));
        _services.Setup(s => s.Properties.UpdateAsync(property));

        // Act & Assert.
        await _handler.Handle(command, new CancellationToken());

        // Assert that the property is missing the deleted items.
        property.Items.Select(i => i.Id).Should().NotContain(items.Select(i => i.Id));
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheAdminDoesntOwnAnOrganization()
    {
        // Prepare
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake(items);
        var employee = EmployeeFaker.Fake(items);
        var organization = OrganizationFaker.Fake(admin: null, property: property, employees: [employee]);

        var command = new DeletePropertyItemsCommand
        {
            Ids = [.. items.Select(i => i.Id)],
            Jwt = new Jwt([], []),
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("You have not created an organization. You must create an organization before modifying your property.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenPropertyIsNotCreated()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake(items);
        var employee = EmployeeFaker.Fake(items);
        var organization = OrganizationFaker.Fake(admin: admin, property: null, employees: [employee]);

        var command = new DeletePropertyItemsCommand
        {
            Ids = [.. items.Select(i => i.Id)],
            Jwt = new Jwt([], []),
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("You have not created a property. You must create a property before modifying its items.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenItemIsNotOfTheProperty()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake();
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, property: property, employees: [employee]);

        var command = new DeletePropertyItemsCommand
        {
            Ids = [.. items.Select(i => i.Id)],
            Jwt = new Jwt([], []),
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync(property);

        var itemQueue = new Queue<PropertyItem>(items);
        _services.Setup(s => s.Properties.Items.GetAsync(It.IsAny<Expression<Func<PropertyItem, bool>>>())).ReturnsAsync(() => itemQueue.Dequeue());

        // Act & Assert.
        var action = async () => await _handler.Handle(command, new CancellationToken());

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("Cannot update a item from a property you do not own.");
    }
}
