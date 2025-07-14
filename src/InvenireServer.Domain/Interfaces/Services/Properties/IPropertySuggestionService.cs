using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Domain.Interfaces.Services.Properties;

public interface IPropertySuggestionService
{
    Task<PropertySuggestion> GetAsync(Expression<Func<PropertySuggestion, bool>> predicate);
    Task CreateAsync(PropertySuggestion suggestion);
    Task UpdateAsync(PropertySuggestion suggestion);
}
