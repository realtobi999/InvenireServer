using System.Text;
using InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;
using InvenireServer.Application.Core.Organizations.Invitations.Commands.ImportFromCsv;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Unit.Helpers;
using MediatR;

namespace InvenireServer.Tests.Unit.Core.Organizations.Invitations.Commands;

/// <summary>
/// Tests for <see cref="ImportFromCsvOrganizationInvitationCommandHandler"/>.
/// </summary>
public class ImportFromCsvOrganizationInvitationCommandHandlerTests : CommandHandlerTester
{
    private readonly Mock<IMediator> _mediator;
    private readonly ImportFromCsvOrganizationInvitationCommandHandler _handler;

    public ImportFromCsvOrganizationInvitationCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new ImportFromCsvOrganizationInvitationCommandHandler(_mediator.Object);
    }

    /// <summary>
    /// Verifies that the handler imports invitations from a valid CSV file.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var email1 = _faker.Internet.Email();
        var email2 = _faker.Internet.Email();
        var payload = $"description,employee_email_address\n{_faker.Lorem.Sentence()},{email1}\n{_faker.Lorem.Sentence()},{email2}";
        var command = new ImportFromCsvOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([]),
            Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)),
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

        captured.Should().HaveCount(2);
        captured.Should().OnlyContain(c => c.Jwt == command.Jwt);
        captured.Select(c => c.EmployeeEmailAddress).Should().ContainInOrder(email1, email2);
    }

    /// <summary>
    /// Verifies that the handler throws when the file contains invalid CSV.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenFileIsCorrupted()
    {
        // Prepare.
        var command = new ImportFromCsvOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([]),
            Stream = new MemoryStream(Encoding.UTF8.GetBytes("description\nINVALID_DATA")),
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
        var email = _faker.Internet.Email();
        var payload = $"description,employee_email_address\n{_faker.Lorem.Sentence()},{email}";
        var command = new ImportFromCsvOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([]),
            Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)),
        };

        // Prepare - mediator.
        _mediator
            .Setup(m => m.Send(It.IsAny<CreateOrganizationInvitationCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFound404Exception("The employee was not found in the system."));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage($"The employee was not found in the system. (key - {email})");
    }

    /// <summary>
    /// Verifies that the handler enriches conflict errors with the employee key.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenInvitationAlreadyExistsForEmployee()
    {
        // Prepare.
        var email = _faker.Internet.Email();
        var payload = $"description,employee_email_address\n{_faker.Lorem.Sentence()},{email}";
        var command = new ImportFromCsvOrganizationInvitationCommand
        {
            Jwt = _jwt.Builder.Build([]),
            Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)),
        };

        // Prepare - mediator.
        _mediator
            .Setup(m => m.Send(It.IsAny<CreateOrganizationInvitationCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Conflict409Exception("The organization already has a invitation for the employee."));

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Conflict409Exception>().WithMessage($"The organization already has a invitation for the employee. (key - {email})");
    }
}
