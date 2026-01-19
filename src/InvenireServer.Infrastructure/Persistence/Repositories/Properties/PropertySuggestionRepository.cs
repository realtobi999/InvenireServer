using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Interfaces.Repositories.Properties;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Properties;

/// <summary>
/// Default implementation of <see cref="IPropertySuggestionRepository"/>.
/// </summary>
public class PropertySuggestionRepository : RepositoryBase<PropertySuggestion>, IPropertySuggestionRepository
{
    public PropertySuggestionRepository(InvenireServerContext context) : base(context)
    {
    }

    /// <summary>
    /// Returns closed suggestions that have expired.
    /// </summary>
    /// <returns>Awaitable task returning expired suggestions.</returns>
    public async Task<IEnumerable<PropertySuggestion>> IndexClosedExpiredAsync()
    {
        var threshold = DateTimeOffset.UtcNow.Add(-PropertySuggestion.EXPIRATION_TIME);
        return await IndexAsync(s => s.Status == PropertySuggestionStatus.DECLINED && s.ResolvedAt <= threshold);
    }

    /// <summary>
    /// Builds a search predicate for property suggestions.
    /// </summary>
    /// <param name="term">Search term to match against suggestion name.</param>
    /// <returns>Filter expression for searching suggestions.</returns>
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

    /// <summary>
    /// Updates a property suggestion entity in the repository.
    /// </summary>
    /// <param name="suggestion">Property suggestion to update.</param>
    public override void Update(PropertySuggestion suggestion)
    {
        suggestion.LastUpdatedAt = DateTimeOffset.UtcNow;
        base.Update(suggestion);
    }
}
