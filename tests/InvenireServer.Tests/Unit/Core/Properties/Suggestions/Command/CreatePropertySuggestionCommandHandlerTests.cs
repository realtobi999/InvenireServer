using System.Text.Json;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Create;
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

public class CreatePropertySuggestionCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly CreatePropertySuggestionCommandHandler _handler;

    public CreatePropertySuggestionCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new CreatePropertySuggestionCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectInstance()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var employee = EmployeeFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake(property: property, employees: [employee]);

        var faker = new Faker();
        var command = new CreatePropertySuggestionCommand
        {
            Id = Guid.NewGuid(),
            Name = faker.Lorem.Sentence(3),
            Description = faker.Lorem.Paragraph(),
            Body = new CreatePropertySuggestionCommand.RequestBody
            {
                CreateCommands = [.. items.Select(i => i.ToCreatePropertyItemCommand())],
                DeleteCommands = [],
                UpdateCommands = []
            },
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == employee.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Suggestion.CreateAsync(It.IsAny<PropertySuggestion>()));

        // Act & Assert.
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert that the suggestion is correctly constructed.
        result.Suggestion.Id.Should().Be(command.Id.ToString());
        result.Suggestion.Name.Should().Be(command.Name);
        result.Suggestion.Description.Should().Be(command.Description);
        result.Suggestion.Feedback.Should().BeNull();
        result.Suggestion.Status.Should().Be(PropertySuggestionStatus.PENDING);
        result.Suggestion.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        result.Suggestion.LastUpdatedAt.Should().BeNull();
        result.Suggestion.ResolvedAt.Should().BeNull();
        result.Suggestion.RequestBody.Should().Be(JsonSerializer.Serialize(command.Body));
        result.Suggestion.PropertyId.Should().Be(property.Id);
        result.Suggestion.EmployeeId.Should().Be(employee.Id);
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenEmployeeDoesntBelongInOrganization()
    {
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var employee = EmployeeFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake(property: property, employees: []);

        var faker = new Faker();
        var command = new CreatePropertySuggestionCommand
        {
            Id = Guid.NewGuid(),
            Name = faker.Lorem.Sentence(3),
            Description = faker.Lorem.Paragraph(),
            Body = new CreatePropertySuggestionCommand.RequestBody
            {
                CreateCommands = [.. items.Select(i => i.ToCreatePropertyItemCommand())],
                DeleteCommands = [],
                UpdateCommands = []
            },
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == employee.OrganizationId)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("You are not part of an organization.");
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenPropertyIsNotCreated()
    {
        // Prepare.
        var items = new List<PropertyItem>();
        for (var _ = 0; _ < 5; _++) items.Add(PropertyItemFaker.Fake());

        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake(property: null, employees: [employee]);

        var faker = new Faker();
        var command = new CreatePropertySuggestionCommand
        {
            Id = Guid.NewGuid(),
            Name = faker.Lorem.Sentence(3),
            Description = faker.Lorem.Paragraph(),
            Body = new CreatePropertySuggestionCommand.RequestBody
            {
                CreateCommands = [.. items.Select(i => i.ToCreatePropertyItemCommand())],
                DeleteCommands = [],
                UpdateCommands = []
            },
            Jwt = new Jwt([], [])
        };

        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.TryGetAsync(o => o.Id == employee.OrganizationId)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetAsync(p => p.OrganizationId == organization.Id)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("Organization you are part of doesn't have a property.");
    }
}
