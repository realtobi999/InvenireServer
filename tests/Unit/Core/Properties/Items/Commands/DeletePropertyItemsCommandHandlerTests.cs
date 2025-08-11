using System.Linq.Expressions;
using InvenireServer.Application.Core.Properties.Items.Commands.Delete;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Properties;
using InvenireServer.Tests.Fakers.Properties.Items;
using InvenireServer.Tests.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Properties.Items.Commands;

public class DeletePropertyItemsCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly DeletePropertyItemsCommandHandler _handler;

    public DeletePropertyItemsCommandHandlerTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _handler = new DeletePropertyItemsCommandHandler(_repositories.Object);
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
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        var itemQueue = new Queue<PropertyItem>(items);
        _repositories.Setup(r => r.Properties.Items.GetAsync(It.IsAny<Expression<Func<PropertyItem, bool>>>())).ReturnsAsync(() => itemQueue.Dequeue());
        _repositories.Setup(r => r.Properties.Items.Delete(It.IsAny<PropertyItem>()));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheAdminIsNotFound()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var command = new DeletePropertyItemsCommand
        {
            Ids = [.. items.Select(i => i.Id)],
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found in the system.");
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
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake(items);
        var employee = EmployeeFaker.Fake(items);
        var organization = OrganizationFaker.Fake(admin: admin, property: null, employees: [employee]);

        var command = new DeletePropertyItemsCommand
        {
            Ids = [.. items.Select(i => i.Id)],
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(s => s.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(s => s.Properties.GetForAsync(organization)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The organization doesn't have a property.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheItemIsNotFound()
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
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Items.GetAsync(It.IsAny<Expression<Func<PropertyItem, bool>>>())).ReturnsAsync((PropertyItem?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The item was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenTheItemIsNotOfTheProperty()
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
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(s => s.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(s => s.Properties.GetForAsync(organization)).ReturnsAsync(property);
        var itemQueue = new Queue<PropertyItem>(items);
        _repositories.Setup(s => s.Properties.Items.GetAsync(It.IsAny<Expression<Func<PropertyItem, bool>>>())).ReturnsAsync(() => itemQueue.Dequeue());

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The item is not from the property.");
    }
}
