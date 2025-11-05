using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;

namespace InvenireServer.Application.Core.Properties.Items.Commands.ImportFromJson;

public class ImportFromJsonPropertyItemsCommandHandler : IRequestHandler<ImportFromJsonPropertyItemsCommand>
{
    private readonly IMediator _mediator;

    public ImportFromJsonPropertyItemsCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

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
