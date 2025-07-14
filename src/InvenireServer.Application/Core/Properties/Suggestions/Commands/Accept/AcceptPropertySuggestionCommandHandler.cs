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

    public async Task Handle(AcceptPropertySuggestionCommand request, CancellationToken _)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt!);
        var organization = await _services.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId) ?? throw new BadRequest400Exception("You do not own a organization.");
        var property = await _services.Properties.TryGetAsync(p => p.OrganizationId == organization.Id) ?? throw new BadRequest400Exception("You have not created a property.");
        var suggestion = await _services.Properties.Suggestion.GetAsync(s => s.Id == request.SuggestionId);

        if (suggestion.PropertyId != property.Id) throw new BadRequest400Exception("The suggestion isn't a part of your property.");
        if (suggestion.Status != PropertySuggestionStatus.PENDING) throw new BadRequest400Exception("The suggestion is already closed or approved.");

        switch (suggestion.RequestType)
        {
            case PropertySuggestionRequestType.CREATE:
                await _mediator.Send(new CreatePropertyItemsCommand
                {
                    Items = JsonSerializer.Deserialize<List<CreatePropertyItemCommand>>(suggestion.RequestBody)!,
                    Jwt = request.Jwt,
                }, _);

                break;
            case PropertySuggestionRequestType.UPDATE:
                await _mediator.Send(new UpdatePropertyItemsCommand
                {
                    Items = JsonSerializer.Deserialize<List<UpdatePropertyItemCommand>>(suggestion.RequestBody)!,
                    Jwt = request.Jwt,
                }, _);

                break;
            case PropertySuggestionRequestType.DELETE:
                await _mediator.Send(new DeletePropertyItemsCommand
                {
                    Ids = JsonSerializer.Deserialize<List<Guid>>(suggestion.RequestBody)!,
                    Jwt = request.Jwt,
                }, _);

                break;
        }

        suggestion.Status = PropertySuggestionStatus.APPROVED;

        await _services.Properties.Suggestion.UpdateAsync(suggestion);
    }
}
