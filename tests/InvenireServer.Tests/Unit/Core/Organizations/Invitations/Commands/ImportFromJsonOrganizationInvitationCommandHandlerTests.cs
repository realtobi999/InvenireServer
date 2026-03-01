using System.Text;
using System.Text.Json;
using InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;
using InvenireServer.Application.Core.Organizations.Invitations.Commands.ImportFromJson;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Unit.Helpers;
using MediatR;

namespace InvenireServer.Tests.Unit.Core.Organizations.Invitations.Commands;

/// <summary>
/// Tests for <see cref="ImportFromJsonOrganizationInvitationCommandHandler"/>.
/// </summary>
public class ImportFromJsonOrganizationInvitationCommandHandlerTests : CommandHandlerTester
{
    private readonly Mock<IMediator> _mediator;
    private readonly ImportFromJsonOrganizationInvitationCommandHandler _handler;

    public ImportFromJsonOrganizationInvitationCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new ImportFromJsonOrganizationInvitationCommandHandler(_mediator.Object);
    }

    /// <summary>
    /// Verifies that the handler imports invitations from a valid JSON file.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var invitations = new List<CreateOrganizationInvitationCommand>
        {
            new() {
                Description = _faker.Lorem.Sentence(),
                EmployeeEmailAddress = _faker.Internet.Email(),
            },
            new() {
                Description = _faker.Lorem.Sentence(),
                EmployeeEmailAddress = _faker.Internet.Email(),
            }
        };
        var command = new ImportFromJsonOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([]),
            Stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(invitations))),
        };
        var captured = new List<CreateOrganizationInvitationCommand>();

        // Prepare - mediator.
        _mediator
            .Setup(m => m.Send(It.IsAny<CreateOrganizationInvitationCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<CreateOrganizationInvitationCommandResult>, CancellationToken>((c, _) => captured.Add((CreateOrganizationInvitationCommand)c))
            .ReturnsAsync(new CreateOrganizationInvitationCommandResult
            {
                Invitation = OrganizationInvitationFaker.Fake(),
            });

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        captured.Should().HaveCount(invitations.Count);
        captured.Should().OnlyContain(c => c.Jwt == command.Jwt);
        captured.Select(c => c.EmployeeEmailAddress).Should().ContainInOrder(invitations.Select(c => c.EmployeeEmailAddress));
    }

    /// <summary>
    /// Verifies that the handler throws when the file contains invalid JSON.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenFileIsCorrupted()
    {
        // Prepare.
        var command = new ImportFromJsonOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([]),
            Stream = new MemoryStream(Encoding.UTF8.GetBytes("{")),
        };

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The file is corrupted or in a bad format.");
    }

    /// <summary>
    /// Verifies that the handler enriches not-found errors with the employee key.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenEmployeeIsNotFound()
    {
        // Prepare.
        var invitation = new CreateOrganizationInvitationCommand
        {
            Description = _faker.Lorem.Sentence(),
            EmployeeEmailAddress = _faker.Internet.Email(),
        };
        var command = new ImportFromJsonOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([]),
            Stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new List<CreateOrganizationInvitationCommand> { invitation }))),
        };

        // Prepare - mediator.
        _mediator
            .Setup(m => m.Send(It.IsAny<CreateOrganizationInvitationCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFound404Exception("The employee was not found in the system."));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage($"The employee was not found in the system. (key - {invitation.EmployeeEmailAddress})");
    }

    /// <summary>
    /// Verifies that the handler enriches conflict errors with the employee key.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenInvitationAlreadyExistsForEmployee()
    {
        // Prepare.
        var invitation = new CreateOrganizationInvitationCommand
        {
            Description = _faker.Lorem.Sentence(),
            EmployeeEmailAddress = _faker.Internet.Email(),
        };
        var command = new ImportFromJsonOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([]),
            Stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new List<CreateOrganizationInvitationCommand> { invitation }))),
        };

        // Prepare - mediator.
        _mediator
            .Setup(m => m.Send(It.IsAny<CreateOrganizationInvitationCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Conflict409Exception("The organization already has a invitation for the employee."));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Conflict409Exception>().WithMessage($"The organization already has a invitation for the employee. (key - {invitation.EmployeeEmailAddress})");
    }
}
