using System.Security.Claims;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Delete;
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

/// <summary>
/// Tests for <see cref="DeletePropertySuggestionCommandHandler"/>.
/// </summary>
public class DeletePropertySuggestionCommandHandlerTests : CommandHandlerTester
{
    private readonly DeletePropertySuggestionCommandHandler _handler;

    public DeletePropertySuggestionCommandHandlerTests()
    {
        _handler = new DeletePropertySuggestionCommandHandler(_repositories.Object);
    }

    /// <summary>
    /// Verifies that the handler deletes a suggestion as an admin.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException_AsAdmin()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, property: property);

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.PropertyId = property.Id;

        var command = new DeletePropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.ADMIN)]),
            SuggestionId = suggestion.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Suggestions.ExecuteDeleteAsync(suggestion)).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();
    }

    /// <summary>
    /// Verifies that the handler throws when the admin is not found.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenAdminIsNotFound()
    {
        // Prepare.
        var suggestion = PropertySuggestionFaker.Fake();
        var command = new DeletePropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.ADMIN)]),
            SuggestionId = suggestion.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found in the system.");
    }

    /// <summary>
    /// Verifies that the handler throws when the admin does not own an organization.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenOrganizationIsNotCreated_ForAdmin()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var suggestion = PropertySuggestionFaker.Fake();
        var command = new DeletePropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.ADMIN)]),
            SuggestionId = suggestion.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin doesn't own a organization.");
    }

    /// <summary>
    /// Verifies that the handler throws when the organization has no property for an admin.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenPropertyIsNotFound_ForAdmin()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake();

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.PropertyId = Guid.NewGuid();

        var command = new DeletePropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.ADMIN)]),
            SuggestionId = suggestion.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The organization doesn't have a property.");
    }

    /// <summary>
    /// Verifies that the handler throws when the suggestion is not part of the property for an admin.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenSuggestionIsNotPartOfProperty_ForAdmin()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, property: property);

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.PropertyId = Guid.NewGuid();

        var command = new DeletePropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.ADMIN)]),
            SuggestionId = suggestion.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The suggestion isn't a part the property.");
    }

    /// <summary>
    /// Verifies that the handler deletes a suggestion as the owning employee.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException_AsEmployee()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake(property: property);

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.PENDING;
        suggestion.EmployeeId = employee.Id;
        suggestion.PropertyId = property.Id;

        var command = new DeletePropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.EMPLOYEE)]),
            SuggestionId = suggestion.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Suggestions.ExecuteDeleteAsync(suggestion)).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();
    }

    /// <summary>
    /// Verifies that the handler throws when the employee is not found.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenEmployeeIsNotFound()
    {
        // Prepare.
        var suggestion = PropertySuggestionFaker.Fake();
        var command = new DeletePropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.EMPLOYEE)]),
            SuggestionId = suggestion.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
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
    public async Task Handle_ThrowsException_WhenOrganizationIsNotCreated_ForEmployee()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.EmployeeId = employee.Id;

        var command = new DeletePropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.EMPLOYEE)]),
            SuggestionId = suggestion.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The employee isn't part of any organization.");
    }

    /// <summary>
    /// Verifies that the handler throws when the organization has no property for an employee.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenPropertyIsNotFound_ForEmployee()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var organization = OrganizationFaker.Fake();

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.EmployeeId = employee.Id;

        var command = new DeletePropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.EMPLOYEE)]),
            SuggestionId = suggestion.Id,
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

    /// <summary>
    /// Verifies that the handler throws when the suggestion was already approved.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenSuggestionIsApproved()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake(property: property);

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.APPROVED;
        suggestion.EmployeeId = employee.Id;
        suggestion.PropertyId = property.Id;

        var command = new DeletePropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.EMPLOYEE)]),
            SuggestionId = suggestion.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("The suggestion was already approved by the admin.");
    }

    /// <summary>
    /// Verifies that the handler throws when the suggestion does not belong to the employee.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenSuggestionIsNotAssignedToEmployee()
    {
        // Prepare.
        var employee1 = EmployeeFaker.Fake();
        var employee2 = EmployeeFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake(property: property);

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.PENDING;
        suggestion.EmployeeId = employee2.Id;
        suggestion.PropertyId = property.Id;

        var command = new DeletePropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.EMPLOYEE)]),
            SuggestionId = suggestion.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Employees.GetAsync(command.Jwt)).ReturnsAsync(employee1);
        _repositories.Setup(r => r.Organizations.GetForAsync(employee1)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Unauthorized401Exception>().WithMessage("The suggestion doesn't belong to the employee.");
    }

    /// <summary>
    /// Verifies that the handler throws when the suggestion is not part of the property for an employee.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenSuggestionIsNotPartOfProperty_ForEmployee()
    {
        // Prepare.
        var employee = EmployeeFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake(property: property);

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.PENDING;
        suggestion.EmployeeId = employee.Id;
        suggestion.PropertyId = Guid.NewGuid();

        var command = new DeletePropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.EMPLOYEE)]),
            SuggestionId = suggestion.Id,
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

    /// <summary>
    /// Verifies that the handler throws when the suggestion is not found.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenSuggestionIsNotFound()
    {
        // Prepare.
        var command = new DeletePropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([new Claim("role", Jwt.Roles.ADMIN)]),
            SuggestionId = Guid.NewGuid(),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync((PropertySuggestion?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The suggestion was not found in the system.");
    }
}
