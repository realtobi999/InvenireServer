using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Domain.Interfaces.Services.Properties.Suggestions;

public interface IPropertySuggestionService
{
    Task<IEnumerable<PropertySuggestion>> IndexClosedExpiredAsync();
    Task<PropertySuggestion> GetAsync(Expression<Func<PropertySuggestion, bool>> predicate);
    Task CreateAsync(PropertySuggestion suggestion);
    Task UpdateAsync(PropertySuggestion suggestion);
    Task DeleteAsync(PropertySuggestion suggestion);
    Task DeleteAsync(IEnumerable<PropertySuggestion> suggestions);
}
