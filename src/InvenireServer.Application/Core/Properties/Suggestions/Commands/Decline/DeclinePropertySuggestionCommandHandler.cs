using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Decline;

public class DeclinePropertySuggestionCommandHandler : IRequestHandler<DeclinePropertySuggestionCommand>
{
    private readonly IServiceManager _services;

    public DeclinePropertySuggestionCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(DeclinePropertySuggestionCommand request, CancellationToken _)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt!);
        var organization = await _services.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId) ?? throw new BadRequest400Exception();
        var property = await _services.Properties.TryGetAsync(p => p.OrganizationId == organization.Id) ?? throw new BadRequest400Exception();
        var suggestion = await _services.Properties.Suggestion.GetAsync(s => s.Id == request.SuggestionId);

        if (suggestion.PropertyId != property.Id) throw new BadRequest400Exception();
        if (suggestion.Status != PropertySuggestionStatus.PENDING) throw new BadRequest400Exception();

        suggestion.Feedback = request.Feedback;
        suggestion.Status = PropertySuggestionStatus.DECLINED;

        await _services.Properties.Suggestion.UpdateAsync(suggestion);
    }
}
