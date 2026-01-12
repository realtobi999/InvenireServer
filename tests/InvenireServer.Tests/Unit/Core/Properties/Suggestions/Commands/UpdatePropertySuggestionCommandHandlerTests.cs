using System.Text.Json;
using InvenireServer.Application.Core.Properties.Suggestions.Commands;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Update;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Properties;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Properties.Suggestions.Commands;

public class UpdatePropertySuggestionCommandHandlerTests : CommandHandlerTester
{
    private readonly UpdatePropertySuggestionCommandHandler _handler;

    public UpdatePropertySuggestionCommandHandlerTests()
    {
        _handler = new UpdatePropertySuggestionCommandHandler(_repositories.Object);
    }

    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var property = PropertyFaker.Fake();

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.PENDING;
        suggestion.EmployeeId = employee.Id;
        suggestion.PropertyId = property.Id;

        var command = new UpdatePropertySuggestionCommand
        {
            SuggestionId = suggestion.Id,
            Description = _faker.Lorem.Paragraph(),
            Payload = new PropertySuggestionPayload
            {
                CreateCommands = [],
                UpdateCommands = [],
                DeleteCommands = []
            },
            Jwt = new Jwt([], []),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Suggestions.ExecuteUpdateAsync(suggestion)).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        suggestion.Description.Should().Be(command.Description);
        suggestion.PayloadString.Should().Be(JsonSerializer.Serialize(command.Payload));
        suggestion.Status.Should().Be(PropertySuggestionStatus.PENDING);
    }

    [Fact]
    public async Task Handle_ThrowsNoException_WhenSuggestionWasDeclined()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var property = PropertyFaker.Fake();

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.DECLINED;
        suggestion.EmployeeId = employee.Id;
        suggestion.PropertyId = property.Id;
        suggestion.Feedback = _faker.Lorem.Sentence();
        suggestion.ResolvedAt = _faker.Date.RecentOffset();

        var command = new UpdatePropertySuggestionCommand
        {
            SuggestionId = suggestion.Id,
            Description = _faker.Lorem.Paragraph(),
            Payload = new PropertySuggestionPayload
            {
                CreateCommands = [],
                UpdateCommands = [],
                DeleteCommands = []
            },
            Jwt = new Jwt([], []),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Suggestions.ExecuteUpdateAsync(suggestion)).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        suggestion.Description.Should().Be(command.Description);
        suggestion.PayloadString.Should().Be(JsonSerializer.Serialize(command.Payload));
        suggestion.Status.Should().Be(PropertySuggestionStatus.PENDING);
        suggestion.Feedback.Should().BeNull();
        suggestion.ResolvedAt.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenSuggestionIsNotFound()
    {
        // Prepare.
        var command = new UpdatePropertySuggestionCommand
        {
            SuggestionId = Guid.NewGuid(),
            Description = _faker.Lorem.Paragraph(),
            Payload = new PropertySuggestionPayload
            {
                CreateCommands = [],
                UpdateCommands = [],
                DeleteCommands = []
            },
            Jwt = new Jwt([], []),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync((PropertySuggestion?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The suggestion was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenEmployeeIsNotFound()
    {
        // Prepare.
        var suggestion = PropertySuggestionFaker.Fake();
        var command = new UpdatePropertySuggestionCommand
        {
            SuggestionId = suggestion.Id,
            Description = _faker.Lorem.Paragraph(),
            Payload = new PropertySuggestionPayload
            {
                CreateCommands = [],
                UpdateCommands = [],
                DeleteCommands = []
            },
            Jwt = new Jwt([], []),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The employee was not found in the system.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenOrganizationIsNotCreated()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.EmployeeId = employee.Id;

        var command = new UpdatePropertySuggestionCommand
        {
            SuggestionId = suggestion.Id,
            Description = _faker.Lorem.Paragraph(),
            Payload = new PropertySuggestionPayload
            {
                CreateCommands = [],
                UpdateCommands = [],
                DeleteCommands = []
            },
            Jwt = new Jwt([], []),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The employee isn't part of any organization.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenPropertyIsNotFound()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake();

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.EmployeeId = employee.Id;

        var command = new UpdatePropertySuggestionCommand
        {
            SuggestionId = suggestion.Id,
            Description = _faker.Lorem.Paragraph(),
            Payload = new PropertySuggestionPayload
            {
                CreateCommands = [],
                UpdateCommands = [],
                DeleteCommands = []
            },
            Jwt = new Jwt([], []),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The organization doesn't have a property.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenSuggestionDoesntBelongToEmployee()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.EmployeeId = Guid.NewGuid();

        var command = new UpdatePropertySuggestionCommand
        {
            SuggestionId = suggestion.Id,
            Description = _faker.Lorem.Paragraph(),
            Payload = new PropertySuggestionPayload
            {
                CreateCommands = [],
                UpdateCommands = [],
                DeleteCommands = []
            },
            Jwt = new Jwt([], []),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync(OrganizationFaker.Fake());
        _repositories.Setup(r => r.Properties.GetForAsync(It.IsAny<Organization>())).ReturnsAsync(PropertyFaker.Fake());

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("The suggestion doesn't belong to the employee.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenSuggestionIsNotPartOfProperty()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var property = PropertyFaker.Fake();

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.EmployeeId = employee.Id;
        suggestion.PropertyId = Guid.NewGuid();

        var command = new UpdatePropertySuggestionCommand
        {
            SuggestionId = suggestion.Id,
            Description = _faker.Lorem.Paragraph(),
            Payload = new PropertySuggestionPayload
            {
                CreateCommands = [],
                UpdateCommands = [],
                DeleteCommands = []
            },
            Jwt = new Jwt([], []),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The suggestion isn't a part of the property.");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenSuggestionIsApproved()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var property = PropertyFaker.Fake();

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.APPROVED;
        suggestion.EmployeeId = employee.Id;
        suggestion.PropertyId = property.Id;

        var command = new UpdatePropertySuggestionCommand
        {
            SuggestionId = suggestion.Id,
            Description = _faker.Lorem.Paragraph(),
            Payload = new PropertySuggestionPayload
            {
                CreateCommands = [],
                UpdateCommands = [],
                DeleteCommands = []
            },
            Jwt = new Jwt([], []),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The suggestion is approved and cannot be updated.");
    }
}
