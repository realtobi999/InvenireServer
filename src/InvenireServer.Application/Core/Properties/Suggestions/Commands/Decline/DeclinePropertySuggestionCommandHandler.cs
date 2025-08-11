using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Decline;

public class DeclinePropertySuggestionCommandHandler : IRequestHandler<DeclinePropertySuggestionCommand>
{
    private readonly IRepositoryManager _repositories;

    public DeclinePropertySuggestionCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task Handle(DeclinePropertySuggestionCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var suggestion = await _repositories.Properties.Suggestions.GetAsync(s => s.Id == request.SuggestionId) ?? throw new NotFound404Exception("The suggestion was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        if (suggestion.PropertyId != property.Id) throw new BadRequest400Exception("The suggestion isn't a part of your property.");
        if (suggestion.Status != PropertySuggestionStatus.PENDING) throw new BadRequest400Exception("The suggestion is already closed or approved.");

        suggestion.Feedback = request.Feedback;
        suggestion.Status = PropertySuggestionStatus.DECLINED;
        suggestion.ResolvedAt = DateTimeOffset.UtcNow;

        _repositories.Properties.Suggestions.Update(suggestion);

        await _repositories.SaveOrThrowAsync();
    }
}
