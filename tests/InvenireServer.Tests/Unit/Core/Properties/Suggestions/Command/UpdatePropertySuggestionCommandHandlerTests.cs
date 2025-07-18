using System.Text.Json;
using InvenireServer.Application.Core.Properties.Suggestions.Commands;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Update;
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

public class UpdatePropertySuggestionCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly UpdatePropertySuggestionCommandHandler _handler;

    public UpdatePropertySuggestionCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new UpdatePropertySuggestionCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_UpdatesSuggestionCorrectly()
    {
        // Prepare.
        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.DECLINED;

        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var employee = EmployeeFaker.Fake(suggestions: [suggestion]);
        var property = PropertyFaker.Fake(suggestions: [suggestion]);
        var organization = OrganizationFaker.Fake(property: property, employees: [employee]);

        var command = new UpdatePropertySuggestionCommand
        {
            Description = new Faker().Lorem.Paragraph(),
            Payload = new PropertySuggestionPayload
            {
                CreateCommands = [.. items.Select(i => i.ToCreatePropertyItemCommand())],
                DeleteCommands = [],
                UpdateCommands = []
            },
            Jwt = new Jwt([], []),
            SuggestionId = suggestion.Id
        };

        _services.Setup(s => s.Properties.Suggestion.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.TryGetForAsync(employee)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Suggestion.UpdateAsync(suggestion));

        // Act & Assert.
        await _handler.Handle(command, CancellationToken.None);

        // Assert that the suggestion is correctly updated.
        suggestion.Description = command.Description;
        suggestion.PayloadString = JsonSerializer.Serialize(command.Payload);
        suggestion.Feedback.Should().BeNull();
        suggestion.Status.Should().Be(PropertySuggestionStatus.PENDING);
        suggestion.ResolvedAt.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenEmployeeIsNotInOrganization()
    {
        // Prepare.
        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.DECLINED;

        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var employee = EmployeeFaker.Fake(suggestions: [suggestion]);
        var property = PropertyFaker.Fake(suggestions: [suggestion]);
        var organization = OrganizationFaker.Fake(property: property, employees: []);

        var command = new UpdatePropertySuggestionCommand
        {
            Description = new Faker().Lorem.Paragraph(),
            Payload = new PropertySuggestionPayload
            {
                CreateCommands = [.. items.Select(i => i.ToCreatePropertyItemCommand())],
                DeleteCommands = [],
                UpdateCommands = []
            },
            Jwt = new Jwt([], []),
            SuggestionId = suggestion.Id
        };

        _services.Setup(s => s.Properties.Suggestion.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.TryGetForAsync(employee)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("You are not part of an organization.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenPropertyIsNotCreated()
    {
        // Prepare.
        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.DECLINED;

        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var employee = EmployeeFaker.Fake(suggestions: [suggestion]);
        var organization = OrganizationFaker.Fake(property: null, employees: [employee]);

        var command = new UpdatePropertySuggestionCommand
        {
            Description = new Faker().Lorem.Paragraph(),
            Payload = new PropertySuggestionPayload
            {
                CreateCommands = [.. items.Select(i => i.ToCreatePropertyItemCommand())],
                DeleteCommands = [],
                UpdateCommands = []
            },
            Jwt = new Jwt([], []),
            SuggestionId = suggestion.Id
        };

        _services.Setup(s => s.Properties.Suggestion.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.TryGetForAsync(employee)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("Organization you are part of doesn't have a property.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenEmployeeDoesntOwnSuggestion()
    {
        // Prepare.
        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.DECLINED;

        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var employee = EmployeeFaker.Fake(suggestions: []);
        var property = PropertyFaker.Fake(suggestions: [suggestion]);
        var organization = OrganizationFaker.Fake(property: property, employees: [employee]);

        var command = new UpdatePropertySuggestionCommand
        {
            Description = new Faker().Lorem.Paragraph(),
            Payload = new PropertySuggestionPayload
            {
                CreateCommands = [.. items.Select(i => i.ToCreatePropertyItemCommand())],
                DeleteCommands = [],
                UpdateCommands = []
            },
            Jwt = new Jwt([], []),
            SuggestionId = suggestion.Id
        };

        _services.Setup(s => s.Properties.Suggestion.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.TryGetForAsync(employee)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Suggestion.UpdateAsync(suggestion));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Unauthorized401Exception>();
    }


    [Fact]
    public async Task Handle_ThrowsExceptionWhenSuggestionIsNotPartOfProperty()
    {
        // Prepare.
        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.DECLINED;

        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var employee = EmployeeFaker.Fake(suggestions: [suggestion]);
        var property = PropertyFaker.Fake(suggestions: []);
        var organization = OrganizationFaker.Fake(property: property, employees: [employee]);

        var command = new UpdatePropertySuggestionCommand
        {
            Description = new Faker().Lorem.Paragraph(),
            Payload = new PropertySuggestionPayload
            {
                CreateCommands = [.. items.Select(i => i.ToCreatePropertyItemCommand())],
                DeleteCommands = [],
                UpdateCommands = []
            },
            Jwt = new Jwt([], []),
            SuggestionId = suggestion.Id
        };

        _services.Setup(s => s.Properties.Suggestion.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.TryGetForAsync(employee)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Suggestion.UpdateAsync(suggestion));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The suggestion isn't a part of the property.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenSuggestionStatusIsApproved()
    {
        // Prepare.
        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.APPROVED;

        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var employee = EmployeeFaker.Fake(suggestions: [suggestion]);
        var property = PropertyFaker.Fake(suggestions: [suggestion]);
        var organization = OrganizationFaker.Fake(property: property, employees: [employee]);

        var command = new UpdatePropertySuggestionCommand
        {
            Description = new Faker().Lorem.Paragraph(),
            Payload = new PropertySuggestionPayload
            {
                CreateCommands = [.. items.Select(i => i.ToCreatePropertyItemCommand())],
                DeleteCommands = [],
                UpdateCommands = []
            },
            Jwt = new Jwt([], []),
            SuggestionId = suggestion.Id
        };

        _services.Setup(s => s.Properties.Suggestion.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.TryGetForAsync(employee)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Suggestion.UpdateAsync(suggestion));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The suggestion is approved and cannot be updated.");
    }
}
