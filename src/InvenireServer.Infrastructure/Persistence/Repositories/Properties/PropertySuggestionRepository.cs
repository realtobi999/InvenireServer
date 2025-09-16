using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Interfaces.Repositories.Properties;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

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

    public Expression<Func<PropertySuggestion, bool>> BuildSearchExpression(string term)
    {
        if (term.StartsWith('\"') && term.EndsWith('\"'))
        {
            term = term.Trim('"');
        }
        else
        {
            term = $"%{term}%";
        }

        return i => EF.Functions.ILike(i.Name, term);
    }

    public override void Update(PropertySuggestion suggestion)
    {
        suggestion.LastUpdatedAt = DateTimeOffset.UtcNow;
        base.Update(suggestion);
    }
}
