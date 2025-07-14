using System.Text.Json;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Application.Core.Properties.Items.Commands.Delete;
using InvenireServer.Application.Core.Properties.Items.Commands.Update;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Accept;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Extensions.Properties;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Properties;
using InvenireServer.Tests.Integration.Fakers.Users;
using MediatR;

namespace InvenireServer.Tests.Unit.Core.Properties.Suggestions.Command;

public class AcceptPropertySuggestionCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<IServiceManager> _services;
    private readonly AcceptPropertySuggestionCommandHandler _handler;

    public AcceptPropertySuggestionCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _services = new Mock<IServiceManager>();
        _handler = new AcceptPropertySuggestionCommandHandler(_mediator.Object, _services.Object);
    }

    [Fact]
    public async Task Handle_AcceptsSuggestionAndCreatesItems()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var suggestion = PropertySuggestionFaker.Fake();
        var employee = EmployeeFaker.Fake(suggestions: [suggestion]);
        var property = PropertyFaker.Fake(suggestions: [suggestion]);
        var organization = OrganizationFaker.Fake(admin: admin, property: property, employees: [employee]);

        suggestion.Status = PropertySuggestionStatus.PENDING;
        suggestion.RequestBody = JsonSerializer.Serialize(items.Select(i => i.ToCreatePropertyItemCommand()).ToList());
        suggestion.RequestType = PropertySuggestionRequestType.CREATE;

        var command = new AcceptPropertySuggestionCommand
        {
            Jwt = new Jwt([], []),
            SuggestionId = suggestion.Id
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Suggestion.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _services.Setup(s => s.Properties.Suggestion.UpdateAsync(It.IsAny<PropertySuggestion>()));

        _mediator.Setup(m => m.Send(It.IsAny<CreatePropertyItemsCommand>(), CancellationToken.None));

        // Act & Assert.
        await _handler.Handle(command, CancellationToken.None);

        suggestion.Status.Should().Be(PropertySuggestionStatus.APPROVED);
    }

    [Fact]
    public async Task Handle_AcceptsSuggestionAndUpdatesItems()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var suggestion = PropertySuggestionFaker.Fake();
        var employee = EmployeeFaker.Fake(suggestions: [suggestion]);
        var property = PropertyFaker.Fake(suggestions: [suggestion], items: items);
        var organization = OrganizationFaker.Fake(admin: admin, property: property, employees: [employee]);

        suggestion.Status = PropertySuggestionStatus.PENDING;
        suggestion.RequestBody = JsonSerializer.Serialize(items.Select(i => new UpdatePropertyItemCommand
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
        }).ToList());
        suggestion.RequestType = PropertySuggestionRequestType.UPDATE;

        var command = new AcceptPropertySuggestionCommand
        {
            Jwt = new Jwt([], []),
            SuggestionId = suggestion.Id
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Suggestion.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _services.Setup(s => s.Properties.Suggestion.UpdateAsync(It.IsAny<PropertySuggestion>()));

        _mediator.Setup(m => m.Send(It.IsAny<UpdatePropertyItemsCommand>(), CancellationToken.None));

        // Act & Assert.
        await _handler.Handle(command, CancellationToken.None);

        suggestion.Status.Should().Be(PropertySuggestionStatus.APPROVED);
    }

    [Fact]
    public async Task Handle_AcceptsSuggestionAndDeleteItems()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var suggestion = PropertySuggestionFaker.Fake();
        var employee = EmployeeFaker.Fake(suggestions: [suggestion]);
        var property = PropertyFaker.Fake(suggestions: [suggestion], items: items);
        var organization = OrganizationFaker.Fake(admin: admin, property: property, employees: [employee]);

        suggestion.Status = PropertySuggestionStatus.PENDING;
        suggestion.RequestBody = JsonSerializer.Serialize(items.Select(i => i.Id).ToList());
        suggestion.RequestType = PropertySuggestionRequestType.DELETE;

        var command = new AcceptPropertySuggestionCommand
        {
            Jwt = new Jwt([], []),
            SuggestionId = suggestion.Id
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Suggestion.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _services.Setup(s => s.Properties.Suggestion.UpdateAsync(It.IsAny<PropertySuggestion>()));

        _mediator.Setup(m => m.Send(It.IsAny<DeletePropertyItemsCommand>(), CancellationToken.None));

        // Act & Assert.
        await _handler.Handle(command, CancellationToken.None);

        suggestion.Status.Should().Be(PropertySuggestionStatus.APPROVED);
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenAdminDoesntOwnOrganization()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var suggestion = PropertySuggestionFaker.Fake();
        var employee = EmployeeFaker.Fake(suggestions: [suggestion]);
        var property = PropertyFaker.Fake(suggestions: [suggestion], items: items);
        var organization = OrganizationFaker.Fake(admin: null, property: property, employees: [employee]);

        suggestion.Status = PropertySuggestionStatus.PENDING;
        suggestion.RequestBody = JsonSerializer.Serialize(items.Select(i => i.Id).ToList());
        suggestion.RequestType = PropertySuggestionRequestType.DELETE;

        var command = new AcceptPropertySuggestionCommand
        {
            Jwt = new Jwt([], []),
            SuggestionId = suggestion.Id
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("You do not own a organization.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenPropertyIsNotCreated()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var suggestion = PropertySuggestionFaker.Fake();
        var employee = EmployeeFaker.Fake(suggestions: [suggestion]);
        var organization = OrganizationFaker.Fake(admin: admin, property: null, employees: [employee]);

        suggestion.Status = PropertySuggestionStatus.PENDING;
        suggestion.RequestBody = JsonSerializer.Serialize(items.Select(i => i.Id).ToList());
        suggestion.RequestType = PropertySuggestionRequestType.DELETE;

        var command = new AcceptPropertySuggestionCommand
        {
            Jwt = new Jwt([], []),
            SuggestionId = suggestion.Id
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("You have not created a property.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenSuggestionIsNotPartOfTheProperty()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var suggestion = PropertySuggestionFaker.Fake();
        var employee = EmployeeFaker.Fake(suggestions: [suggestion]);
        var property = PropertyFaker.Fake(suggestions: [], items: items);
        var organization = OrganizationFaker.Fake(admin: admin, property: property, employees: [employee]);

        suggestion.Status = PropertySuggestionStatus.PENDING;
        suggestion.RequestBody = JsonSerializer.Serialize(items.Select(i => i.Id).ToList());
        suggestion.RequestType = PropertySuggestionRequestType.DELETE;

        var command = new AcceptPropertySuggestionCommand
        {
            Jwt = new Jwt([], []),
            SuggestionId = suggestion.Id
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Suggestion.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The suggestion isn't a part of your property.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenStatusIsNotPending()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var admin = AdminFaker.Fake();
        var suggestion = PropertySuggestionFaker.Fake();
        var employee = EmployeeFaker.Fake(suggestions: [suggestion]);
        var property = PropertyFaker.Fake(suggestions: [suggestion], items: items);
        var organization = OrganizationFaker.Fake(admin: admin, property: property, employees: [employee]);

        suggestion.Status = PropertySuggestionStatus.DECLINED;
        suggestion.RequestBody = JsonSerializer.Serialize(items.Select(i => i.Id).ToList());
        suggestion.RequestType = PropertySuggestionRequestType.DELETE;

        var command = new AcceptPropertySuggestionCommand
        {
            Jwt = new Jwt([], []),
            SuggestionId = suggestion.Id
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Suggestion.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The suggestion is already closed or approved.");
    }
}
