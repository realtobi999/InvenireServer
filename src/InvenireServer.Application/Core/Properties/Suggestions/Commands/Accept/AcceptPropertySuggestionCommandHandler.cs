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
    private readonly IServiceManager _services;

    public AcceptPropertySuggestionCommandHandler(IMediator mediator, IServiceManager services)
    {
        _mediator = mediator;
        _services = services;
    }

    public async Task Handle(AcceptPropertySuggestionCommand request, CancellationToken ct)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt!);
        var suggestion = await _services.Properties.Suggestion.GetAsync(s => s.Id == request.SuggestionId);
        var organization = await _services.Organizations.TryGetForAsync(admin) ?? throw new BadRequest400Exception("You do not own a organization.");
        var property = await _services.Properties.TryGetForAsync(organization) ?? throw new BadRequest400Exception("You have not created a property.");

        if (suggestion.PropertyId != property.Id) throw new BadRequest400Exception("The suggestion isn't a part of your property.");
        if (suggestion.Status != PropertySuggestionStatus.PENDING) throw new BadRequest400Exception("The suggestion is already closed or approved.");

        var payload = JsonSerializer.Deserialize<PropertySuggestionPayload>(suggestion.PayloadString);
        if (payload!.CreateCommands.Count != 0)
        {
            await _mediator.Send(new CreatePropertyItemsCommand
            {
                Items = payload.CreateCommands,
                Jwt = request.Jwt,
            }, _);
        }
        if (payload!.UpdateCommands.Count != 0)
        {
            await _mediator.Send(new UpdatePropertyItemsCommand
            {
                Items = payload.UpdateCommands,
                Jwt = request.Jwt,
            }, _);

        }
        if (payload!.DeleteCommands.Count != 0)
        {
            await _mediator.Send(new DeletePropertyItemsCommand
            {
                Ids = payload.DeleteCommands,
                Jwt = request.Jwt,
            }, _);

        }

        suggestion.Status = PropertySuggestionStatus.APPROVED;
        suggestion.ResolvedAt = DateTimeOffset.UtcNow;

        await _services.Properties.Suggestion.UpdateAsync(suggestion);
    }
}
