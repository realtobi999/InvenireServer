using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Interfaces.Repositories.Properties;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Properties;

public class PropertyRepository : RepositoryBase<Property>, IPropertyRepository
{
    public PropertyRepository(InvenireServerContext context) : base(context)
    {
        Items = new PropertyItemRepository(context);
    }

    public IPropertyItemRepository Items { get; }
}