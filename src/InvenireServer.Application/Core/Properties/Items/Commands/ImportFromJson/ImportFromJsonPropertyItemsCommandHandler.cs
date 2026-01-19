using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;

namespace InvenireServer.Application.Core.Properties.Items.Commands.ImportFromJson;

/// <summary>
/// Handler for the request to import property items from JSON.
/// </summary>
public class ImportFromJsonPropertyItemsCommandHandler : IRequestHandler<ImportFromJsonPropertyItemsCommand>
{
    private readonly IMediator _mediator;

    public ImportFromJsonPropertyItemsCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Handles the request to import property items from JSON.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task Handle(ImportFromJsonPropertyItemsCommand request, CancellationToken ct)
    {
        var command = JsonSerializer.Deserialize<CreatePropertyItemsCommand>(await new StreamReader(request.Stream).ReadToEndAsync(ct));

        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request file is missing or invalid.")]);

        command = command with
        {
            Jwt = request.Jwt
        };

        await _mediator.Send(command, ct);
    }
}
