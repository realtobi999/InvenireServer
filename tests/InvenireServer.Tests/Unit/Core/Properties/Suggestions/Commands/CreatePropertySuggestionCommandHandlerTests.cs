using System.Text.Json;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Application.Core.Properties.Suggestions.Commands;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Create;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Properties;
using InvenireServer.Tests.Fakers.Properties.Items;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Extensions.Properties;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Properties.Suggestions.Commands;

/// <summary>
/// Tests for <see cref="CreatePropertySuggestionCommandHandler"/>.
/// </summary>
public class CreatePropertySuggestionCommandHandlerTests : CommandHandlerTester
{
    private readonly CreatePropertySuggestionCommandHandler _handler;

    public CreatePropertySuggestionCommandHandlerTests()
    {
        _handler = new CreatePropertySuggestionCommandHandler(_repositories.Object);
    }

    /// <summary>
    /// Verifies that the handler creates a pending suggestion for the employee.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var property = PropertyFaker.Fake();
        var payload = new PropertySuggestionPayload
        {
            CreateCommands = [PropertyItemFaker.Fake().ToCreatePropertyItemCommand()],
            UpdateCommands = [],
            DeleteCommands = []
        };
        var command = new CreatePropertySuggestionCommand
        {
            Name = _faker.Lorem.Sentence(),
            Description = _faker.Lorem.Paragraph(),
            Payload = payload,
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Suggestions.ExecuteCreateAsync(It.IsAny<PropertySuggestion>())).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        var result = await action.Invoke();
        result.Suggestion.Should().NotBeNull();
        result.Suggestion.Id.Should().NotBeEmpty();
        result.Suggestion.Name.Should().Be(command.Name);
        result.Suggestion.Description.Should().Be(command.Description);
        result.Suggestion.Feedback.Should().BeNull();
        result.Suggestion.PayloadString.Should().Be(JsonSerializer.Serialize(command.Payload));
        result.Suggestion.Status.Should().Be(PropertySuggestionStatus.PENDING);
        result.Suggestion.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        result.Suggestion.LastUpdatedAt.Should().BeNull();
        result.Suggestion.ResolvedAt.Should().BeNull();
        result.Suggestion.PropertyId.Should().Be(property.Id);
        result.Suggestion.EmployeeId.Should().Be(employee.Id);
    }

    /// <summary>
    /// Verifies that the handler uses the provided suggestion id.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException_WhenIdIsProvided()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var property = PropertyFaker.Fake();
        var command = new CreatePropertySuggestionCommand
        {
            Id = Guid.NewGuid(),
            Name = _faker.Lorem.Sentence(),
            Description = _faker.Lorem.Paragraph(),
            Payload = new PropertySuggestionPayload
            {
                CreateCommands = [],
                UpdateCommands = [],
                DeleteCommands = []
            },
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Suggestions.ExecuteCreateAsync(It.IsAny<PropertySuggestion>())).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        var result = await action.Invoke();
        result.Suggestion.Id.Should().Be(command.Id.ToString());
        result.Suggestion.PropertyId.Should().Be(property.Id);
        result.Suggestion.EmployeeId.Should().Be(employee.Id);
    }

    /// <summary>
    /// Verifies that the handler throws when the employee is not found.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenEmployeeIsNotFound()
    {
        // Prepare.
        var command = new CreatePropertySuggestionCommand
        {
            Name = _faker.Lorem.Sentence(),
            Description = _faker.Lorem.Paragraph(),
            Payload = new PropertySuggestionPayload
            {
                CreateCommands = [],
                UpdateCommands = [],
                DeleteCommands = []
            },
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The employee was not found in the system.");
    }

    /// <summary>
    /// Verifies that the handler throws when the employee is not part of any organization.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenOrganizationIsNotCreated()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var command = new CreatePropertySuggestionCommand
        {
            Name = _faker.Lorem.Sentence(),
            Description = _faker.Lorem.Paragraph(),
            Payload = new PropertySuggestionPayload
            {
                CreateCommands = [],
                UpdateCommands = [],
                DeleteCommands = []
            },
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The employee isn't part of any organization.");
    }

    /// <summary>
    /// Verifies that the handler throws when the organization has no property.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenPropertyIsNotFound()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var command = new CreatePropertySuggestionCommand
        {
            Name = _faker.Lorem.Sentence(),
            Description = _faker.Lorem.Paragraph(),
            Payload = new PropertySuggestionPayload
            {
                CreateCommands = [],
                UpdateCommands = [],
                DeleteCommands = []
            },
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The organization doesn't have a property.");
    }
}
