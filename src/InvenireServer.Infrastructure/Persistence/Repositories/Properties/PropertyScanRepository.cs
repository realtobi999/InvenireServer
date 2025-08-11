using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Interfaces.Repositories.Properties;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Properties;

public class PropertyScanRepository : RepositoryBase<PropertyScan>, IPropertyScanRepository
{
    public PropertyScanRepository(InvenireServerContext context) : base(context)
    {
    }

    public async Task<PropertyScan?> GetInProgressForAsync(Property property)
    {
        return await GetAsync(s => s.PropertyId == property.Id && s.Status == PropertyScanStatus.IN_PROGRESS);
    }

    public async Task<IEnumerable<PropertyScan>> IndexInProgressForAsync(Property property)
    {
        return await IndexAsync(s => s.PropertyId == property.Id && s.Status == PropertyScanStatus.IN_PROGRESS);
    }
}
