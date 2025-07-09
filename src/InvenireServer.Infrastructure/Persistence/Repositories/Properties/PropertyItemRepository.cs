using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Interfaces.Repositories.Properties;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Properties;

public class PropertyItemRepository : RepositoryBase<PropertyItem>, IPropertyItemRepository
{
    public PropertyItemRepository(InvenireServerContext context) : base(context)
    {
    }
}