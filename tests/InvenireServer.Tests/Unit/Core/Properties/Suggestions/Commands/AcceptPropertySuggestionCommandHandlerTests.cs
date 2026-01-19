using System.Text.Json;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Application.Core.Properties.Items.Commands.Delete;
using InvenireServer.Application.Core.Properties.Items.Commands.Update;
using InvenireServer.Application.Core.Properties.Suggestions.Commands;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Accept;
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
using MediatR;

namespace InvenireServer.Tests.Unit.Core.Properties.Suggestions.Commands;

/// <summary>
/// Tests for <see cref="AcceptPropertySuggestionCommandHandler"/>.
/// </summary>
public class AcceptPropertySuggestionCommandHandlerTests : CommandHandlerTester
{
    private readonly Mock<IMediator> _mediator;
    private readonly AcceptPropertySuggestionCommandHandler _handler;

    public AcceptPropertySuggestionCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new AcceptPropertySuggestionCommandHandler(_mediator.Object, _repositories.Object);
    }

    /// <summary>
    /// Verifies that the handler approves a pending suggestion and records resolution data.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var property = PropertyFaker.Fake();
        var organization = OrganizationFaker.Fake(admin: admin, property: property);
        var item = PropertyItemFaker.Fake();

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.PENDING;
        suggestion.PropertyId = property.Id;
        suggestion.PayloadString = JsonSerializer.Serialize(new PropertySuggestionPayload
        {
            CreateCommands = [(PropertyItemFaker.Fake().ToCreatePropertyItemCommand())],
            UpdateCommands =
            [
                new UpdatePropertyItemCommand
                {
                    Id = item.Id,
                    InventoryNumber = item.InventoryNumber,
                    RegistrationNumber = item.RegistrationNumber,
                    Name = item.Name,
                    Price = item.Price,
                    SerialNumber = item.SerialNumber,
                    DateOfPurchase = item.DateOfPurchase,
                    DateOfSale = item.DateOfSale,
                    Location = new UpdatePropertyItemCommandLocation
                    {
                        Room = item.Location.Room,
                        Building = item.Location.Building,
                        AdditionalNote = item.Location.AdditionalNote
                    },
                    Description = item.Description,
                    DocumentNumber = item.DocumentNumber,
                    EmployeeId = item.EmployeeId
                }
            ],
            DeleteCommands = [Guid.NewGuid()]
        });

        var command = new AcceptPropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([]),
            SuggestionId = suggestion.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Suggestions.ExecuteUpdateAsync(suggestion)).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        suggestion.Status.Should().Be(PropertySuggestionStatus.APPROVED);
        suggestion.ResolvedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// Verifies that the handler throws when the admin is not found.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenAdminIsNotFound()
    {
        // Prepare.
        var command = new AcceptPropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([]),
            SuggestionId = Guid.NewGuid(),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found in the system.");
    }

    /// <summary>
    /// Verifies that the handler throws when the suggestion is not found.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenSuggestionIsNotFound()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new AcceptPropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([]),
            SuggestionId = Guid.NewGuid(),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync((PropertySuggestion?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The suggestion was not found in the system.");
    }

    /// <summary>
    /// Verifies that the handler throws when the admin does not own an organization.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenOrganizationIsNotCreated()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var suggestion = PropertySuggestionFaker.Fake();
        var command = new AcceptPropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([]),
            SuggestionId = suggestion.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin doesn't own a organization.");
    }

    /// <summary>
    /// Verifies that the handler throws when the organization has no property.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenPropertyIsNotFound()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake();

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.PENDING;

        var command = new AcceptPropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([]),
            SuggestionId = suggestion.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The organization doesn't have a property.");
    }

    /// <summary>
    /// Verifies that the handler throws when the suggestion is not part of the property.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenSuggestionIsNotPartOfProperty()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var property = PropertyFaker.Fake();

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.PENDING;
        suggestion.PropertyId = Guid.NewGuid();

        var command = new AcceptPropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([]),
            SuggestionId = suggestion.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The suggestion isn't part of the property.");
    }

    /// <summary>
    /// Verifies that the handler throws when the suggestion is already closed.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenSuggestionIsClosed()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var property = PropertyFaker.Fake();

        var suggestion = PropertySuggestionFaker.Fake();
        suggestion.Status = PropertySuggestionStatus.APPROVED;
        suggestion.PropertyId = property.Id;

        var command = new AcceptPropertySuggestionCommand
        {
            Jwt = _jwt.Builder.Build([]),
            SuggestionId = suggestion.Id,
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Properties.Suggestions.GetAsync(s => s.Id == command.SuggestionId)).ReturnsAsync(suggestion);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The suggestion is already closed or approved.");
    }
}
