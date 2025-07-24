using System.Threading.Tasks;
using InvenireServer.Application.Interfaces.Repositories.Properties;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Properties;

public class PropertyScanRepository : RepositoryBase<PropertyScan>, IPropertyScanRepository
{
    public PropertyScanRepository(InvenireServerContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PropertyScan>> IndexInProgressAsync(Property property)
    {
        return await IndexAsync(s => s.PropertyId == property.Id && s.Status == PropertyScanStatus.IN_PROGRESS);
    }
}
