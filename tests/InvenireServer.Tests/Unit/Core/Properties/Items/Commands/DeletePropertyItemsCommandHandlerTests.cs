using System.Linq.Expressions;
using InvenireServer.Application.Core.Properties.Items.Commands.Delete;
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

        _repositories.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(s => s.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(s => s.Properties.GetForAsync(organization)).ReturnsAsync(property);
        var itemQueue = new Queue<PropertyItem>(items);
        _repositories.Setup(s => s.Properties.Items.GetAsync(It.IsAny<Expression<Func<PropertyItem, bool>>>())).ReturnsAsync(() => itemQueue.Dequeue());
        _repositories.Setup(s => s.Properties.Items.Delete(It.IsAny<PropertyItem>()));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();
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

        _repositories.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

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
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

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
            Jwt = new Jwt([], [])
        };

        _repositories.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);

        var itemQueue = new Queue<PropertyItem>(items);
        _repositories.Setup(s => s.Properties.Items.GetAsync(It.IsAny<Expression<Func<PropertyItem, bool>>>())).ReturnsAsync(() => itemQueue.Dequeue());

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("Cannot update a item from a property you do not own.");
    }
}
