using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Interfaces.Repositories.Properties;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Properties;

public class PropertySuggestionRepository : RepositoryBase<PropertySuggestion>, IPropertySuggestionRepository
{
    public PropertySuggestionRepository(InvenireServerContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PropertySuggestion>> IndexClosedExpiredAsync()
    {
        var threshold = DateTimeOffset.UtcNow.Add(-PropertySuggestion.EXPIRATION_TIME);
        return await IndexAsync(s => s.Status == PropertySuggestionStatus.DECLINED && s.ResolvedAt <= threshold);
    }

    public override void Update(PropertySuggestion suggestion)
    {
        suggestion.LastUpdatedAt = DateTimeOffset.UtcNow;
        base.Update(suggestion);
    }
}
