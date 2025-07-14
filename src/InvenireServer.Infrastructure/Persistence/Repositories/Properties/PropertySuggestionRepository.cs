using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Interfaces.Repositories.Properties;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Properties;

public class PropertySuggestionRepository : RepositoryBase<PropertySuggestion>, IPropertySuggestionRepository
{
    public PropertySuggestionRepository(InvenireServerContext context) : base(context)
    {
    }
}
