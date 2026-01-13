using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Interfaces.Repositories.Properties;

/// <summary>
/// Defines a repository for property suggestions.
/// </summary>
public interface IPropertySuggestionRepository : IRepositoryBase<PropertySuggestion>
{
    /// <summary>
    /// Returns closed suggestions that have expired.
    /// </summary>
    /// <returns>Awaitable task returning expired suggestions.</returns>
    Task<IEnumerable<PropertySuggestion>> IndexClosedExpiredAsync();

    /// <summary>
    /// Builds a search predicate for property suggestions.
    /// </summary>
    /// <param name="term">Search term to match against suggestion name.</param>
    /// <returns>Filter expression for searching suggestions.</returns>
    Expression<Func<PropertySuggestion, bool>> BuildSearchExpression(string term);
}
