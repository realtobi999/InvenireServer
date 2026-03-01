using System.Text.Json;
using InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.ImportFromJson;

/// <summary>
/// Handler for the request to import organization invitations from JSON.
/// </summary>
public class ImportFromJsonOrganizationInvitationCommandHandler : IRequestHandler<ImportFromJsonOrganizationInvitationCommand>
{
    private readonly IMediator _mediator;

    public ImportFromJsonOrganizationInvitationCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Handles the request to import organization invitations from JSON.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task Handle(ImportFromJsonOrganizationInvitationCommand request, CancellationToken ct)
    {
        // ensure the stream is at position 0 before reading.
        if (request.Stream.CanSeek)
            request.Stream.Position = 0;

        var commands = (List<CreateOrganizationInvitationCommand>?)null;
        try
        {
            commands = JsonSerializer.Deserialize<List<CreateOrganizationInvitationCommand>>(await new StreamReader(request.Stream).ReadToEndAsync(ct))!;
        }
        catch
        {
            throw new BadRequest400Exception("The file is corrupted or in a bad format.");
        }

        // using a normal for-loop instead of foreach because we need to replace
        // items in the collection (foreach iteration variables are read-only).
        for (int i = 0; i < commands.Count; i++)
        {
            commands[i] = commands[i] with
            {
                Jwt = request.Jwt
            };

            // enrich the exception messages with the email address of the employee.
            try
            {
                await _mediator.Send(commands[i], ct);
            }
            catch (NotFound404Exception exception)
            {
                if (exception.Message == "The employee was not found in the system.")
                {
                    throw new NotFound404Exception(exception.Message + $" (key - {commands[i].EmployeeEmailAddress})", exception);
                }
            }
            catch (Conflict409Exception exception)
            {
                if (exception.Message == "The organization already has a invitation for the employee.")
                {
                    throw new Conflict409Exception(exception.Message + $" (key - {commands[i].EmployeeEmailAddress})", exception);
                }
            }
        }
    }
}
