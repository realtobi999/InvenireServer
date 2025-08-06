using InvenireServer.Application.Core.Properties.Suggestions.Commands.Delete;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Properties;
using InvenireServer.Tests.Fakers.Users;

namespace InvenireServer.Tests.Unit.Core.Properties.Suggestions.Command;

public class DeletePropertySuggestionCommandHandlerTests
{
    private readonly Mock<IServiceManager> _services;
    private readonly DeletePropertySuggestionCommandHandler _handler;

    public DeletePropertySuggestionCommandHandlerTests()
    {
        _services = new Mock<IServiceManager>();
        _handler = new DeletePropertySuggestionCommandHandler(_services.Object);
    }

    [Fact]
    public async Task Handle_DeletesSuggestionWithoutException()
    {
        // Prepare.
        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.DECLINED;

        var employee = EmployeeFaker.Fake(suggestions: [suggestion]);
        var property = PropertyFaker.Fake(suggestions: [suggestion]);
        var organization = OrganizationFaker.Fake(property: property, employees: [employee]);

        var command = new DeletePropertySuggestionCommand
        {
            Jwt = new Jwt([], [new("role", Jwt.Roles.EMPLOYEE)]),
            SuggestionId = suggestion.Id
        };

        _services.Setup(s => s.Properties.Suggestion.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.TryGetForAsync(employee)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Suggestion.DeleteAsync(suggestion));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenEmployeeIsNotInOrganization()
    {
        // Prepare.
        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.DECLINED;

        var employee = EmployeeFaker.Fake(suggestions: [suggestion]);
        var property = PropertyFaker.Fake(suggestions: [suggestion]);
        var organization = OrganizationFaker.Fake(property: property, employees: []);

        var command = new DeletePropertySuggestionCommand
        {
            Jwt = new Jwt([], [new("role", Jwt.Roles.EMPLOYEE)]),
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
        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.DECLINED;

        var employee = EmployeeFaker.Fake(suggestions: [suggestion]);
        var organization = OrganizationFaker.Fake(property: null, employees: [employee]);

        var command = new DeletePropertySuggestionCommand
        {
            Jwt = new Jwt([], [new("role", Jwt.Roles.EMPLOYEE)]),
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
    public async Task Handle_ThrowsExceptionWhenSuggestionStatusIsApproved()
    {
        // Prepare.
        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.APPROVED;

        var employee = EmployeeFaker.Fake(suggestions: [suggestion]);
        var property = PropertyFaker.Fake(suggestions: [suggestion]);
        var organization = OrganizationFaker.Fake(property: property, employees: [employee]);

        var command = new DeletePropertySuggestionCommand
        {
            Jwt = new Jwt([], [new("role", Jwt.Roles.EMPLOYEE)]),
            SuggestionId = suggestion.Id
        };

        _services.Setup(s => s.Properties.Suggestion.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.TryGetForAsync(employee)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<Unauthorized401Exception>();
    }

    [Fact]
    public async Task Handle_ThrowsExceptionWhenEmployeeDoesntOwnSuggestion()
    {
        // Prepare.
        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.DECLINED;

        var employee = EmployeeFaker.Fake(suggestions: []);
        var property = PropertyFaker.Fake(suggestions: [suggestion]);
        var organization = OrganizationFaker.Fake(property: property, employees: [employee]);

        var command = new DeletePropertySuggestionCommand
        {
            Jwt = new Jwt([], [new("role", Jwt.Roles.EMPLOYEE)]),
            SuggestionId = suggestion.Id
        };

        _services.Setup(s => s.Properties.Suggestion.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.TryGetForAsync(employee)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Suggestion.DeleteAsync(suggestion));

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

        var employee = EmployeeFaker.Fake(suggestions: [suggestion]);
        var property = PropertyFaker.Fake(suggestions: []);
        var organization = OrganizationFaker.Fake(property: property, employees: [employee]);

        var command = new DeletePropertySuggestionCommand
        {
            Jwt = new Jwt([], [new("role", Jwt.Roles.EMPLOYEE)]),
            SuggestionId = suggestion.Id
        };

        _services.Setup(s => s.Properties.Suggestion.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _services.Setup(s => s.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _services.Setup(s => s.Organizations.TryGetForAsync(employee)).ReturnsAsync(organization);
        _services.Setup(s => s.Properties.TryGetForAsync(organization)).ReturnsAsync(property);
        _services.Setup(s => s.Properties.Suggestion.DeleteAsync(suggestion));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The suggestion isn't a part of the property.");
    }
}
