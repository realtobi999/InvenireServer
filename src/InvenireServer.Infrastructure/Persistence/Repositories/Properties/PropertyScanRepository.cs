using System.Threading.Tasks;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Interfaces.Repositories.Properties;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Properties;

public class PropertyScanRepository : RepositoryBase<PropertyScan>, IPropertyScanRepository
{
    public PropertyScanRepository(InvenireServerContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PropertyScan>> IndexActiveAsync()
    {
        return await IndexAsync(s => !s.ClosedAt.HasValue);
    }
}
