using System.Text.Json;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Application.Core.Properties.Items.Commands.Delete;
using InvenireServer.Application.Core.Properties.Items.Commands.Update;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Accept;

public class AcceptPropertySuggestionCommandHandler : IRequestHandler<AcceptPropertySuggestionCommand>
{
    private readonly IMediator _mediator;
    private readonly IRepositoryManager _repositories;

    public AcceptPropertySuggestionCommandHandler(IMediator mediator, IRepositoryManager repositories)
    {
        _mediator = mediator;
        _repositories = repositories;
    }

    public async Task Handle(AcceptPropertySuggestionCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var suggestion = await _repositories.Properties.Suggestions.GetAsync(s => s.Id == request.SuggestionId) ?? throw new NotFound404Exception("The suggestion was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        if (suggestion.PropertyId != property.Id) throw new BadRequest400Exception("The suggestion isn't part of the property.");
        if (suggestion.Status != PropertySuggestionStatus.PENDING) throw new BadRequest400Exception("The suggestion is already closed or approved.");

        var payload = JsonSerializer.Deserialize<PropertySuggestionPayload>(suggestion.PayloadString);
        if (payload!.CreateCommands.Count != 0)
        {
            await _mediator.Send(new CreatePropertyItemsCommand
            {
                Items = payload.CreateCommands,
                Jwt = request.Jwt,
            }, ct);
        }
        if (payload!.UpdateCommands.Count != 0)
        {
            await _mediator.Send(new UpdatePropertyItemsCommand
            {
                Items = payload.UpdateCommands,
                Jwt = request.Jwt,
            }, ct);

        }
        if (payload!.DeleteCommands.Count != 0)
        {
            await _mediator.Send(new DeletePropertyItemsCommand
            {
                Ids = payload.DeleteCommands,
                Jwt = request.Jwt,
            }, ct);

        }

        suggestion.Status = PropertySuggestionStatus.APPROVED;
        suggestion.ResolvedAt = DateTimeOffset.UtcNow;

        await _repositories.Properties.Suggestions.ExecuteUpdateAsync(suggestion);
    }
}
