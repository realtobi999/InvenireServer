using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Interfaces.Repositories.Properties;

public interface IPropertySuggestionRepository : IRepositoryBase<PropertySuggestion>
{
    Task<IEnumerable<PropertySuggestion>> IndexClosedExpiredAsync();
}