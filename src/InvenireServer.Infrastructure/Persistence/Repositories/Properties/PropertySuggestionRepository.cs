using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Interfaces.Repositories.Properties;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Properties;

public class PropertySuggestionRepository : RepositoryBase<PropertySuggestion>, IPropertySuggestionRepository
{
    public PropertySuggestionRepository(InvenireServerContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PropertySuggestion>> IndexClosedAsync()
    {
        var threshold = DateTimeOffset.UtcNow.Add(-PropertySuggestion.DECLINED_THRESHOLD);
        return await IndexAsync(s => s.Status == PropertySuggestionStatus.DECLINED && s.CreatedAt <= threshold);
    }
}
