using InvenireServer.Application.Interfaces.Repositories.Properties;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Properties;

public class PropertyItemRepository : RepositoryBase<PropertyItem>, IPropertyItemRepository
{
    public PropertyItemRepository(InvenireServerContext context) : base(context)
    {
    }
}