using System.Text.Json;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Create;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Decline;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Extensions.Properties;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Properties;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Properties.Suggestions.Command;

public class DeclinePropertySuggestionCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly DeclinePropertySuggestionCommandHandler _handler;

    public DeclinePropertySuggestionCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new DeclinePropertySuggestionCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_DeclinesSuggestion()
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
        suggestion.RequestBody = JsonSerializer.Serialize(new CreatePropertySuggestionCommand.RequestBody
        {
            DeleteCommands = [],
            UpdateCommands = [],
            CreateCommands = [.. items.Select(i => i.ToCreatePropertyItemCommand())],
        });

        var command = new DeclinePropertySuggestionCommand
        {
            Feedback = new Faker().Lorem.Sentence(),
            Jwt = new Jwt([], []),
            SuggestionId = suggestion.Id,
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Suggestion.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _services.Setup(s => s.Properties.Suggestion.UpdateAsync(It.IsAny<PropertySuggestion>()));

        // Act & Assert.
        await _handler.Handle(command, CancellationToken.None);

        suggestion.Feedback.Should().Be(command.Feedback);
        suggestion.Status.Should().Be(PropertySuggestionStatus.DECLINED);
        suggestion.ResolvedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
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
        var property = PropertyFaker.Fake(suggestions: [suggestion]);
        var organization = OrganizationFaker.Fake(admin: admin, property: property, employees: [employee]);

        suggestion.Status = PropertySuggestionStatus.PENDING;
        suggestion.RequestBody = JsonSerializer.Serialize(new CreatePropertySuggestionCommand.RequestBody
        {
            DeleteCommands = [],
            UpdateCommands = [],
            CreateCommands = [.. items.Select(i => i.ToCreatePropertyItemCommand())],
        });

        var command = new DeclinePropertySuggestionCommand
        {
            Feedback = new Faker().Lorem.Sentence(),
            Jwt = new Jwt([], []),
            SuggestionId = suggestion.Id,
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Properties.Suggestion.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync((Organization?)null);

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
        suggestion.RequestBody = JsonSerializer.Serialize(new CreatePropertySuggestionCommand.RequestBody
        {
            DeleteCommands = [],
            UpdateCommands = [],
            CreateCommands = [.. items.Select(i => i.ToCreatePropertyItemCommand())],
        });

        var command = new DeclinePropertySuggestionCommand
        {
            Feedback = new Faker().Lorem.Sentence(),
            Jwt = new Jwt([], []),
            SuggestionId = suggestion.Id,
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Properties.Suggestion.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync((Property?)null);


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
        suggestion.RequestBody = JsonSerializer.Serialize(new CreatePropertySuggestionCommand.RequestBody
        {
            DeleteCommands = [],
            UpdateCommands = [],
            CreateCommands = [.. items.Select(i => i.ToCreatePropertyItemCommand())],
        });

        var command = new DeclinePropertySuggestionCommand
        {
            Feedback = new Faker().Lorem.Sentence(),
            Jwt = new Jwt([], []),
            SuggestionId = suggestion.Id,
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
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

        suggestion.Status = PropertySuggestionStatus.APPROVED;
        suggestion.RequestBody = JsonSerializer.Serialize(new CreatePropertySuggestionCommand.RequestBody
        {
            DeleteCommands = [],
            UpdateCommands = [],
            CreateCommands = [.. items.Select(i => i.ToCreatePropertyItemCommand())],
        });

        var command = new DeclinePropertySuggestionCommand
        {
            Feedback = new Faker().Lorem.Sentence(),
            Jwt = new Jwt([], []),
            SuggestionId = suggestion.Id,
        };

        _services.Setup(s => s.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _services.Setup(s => s.Organizations.TryGetForAsync(admin)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Suggestion.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The suggestion is already closed or approved.");
    }
}
