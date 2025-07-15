using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Domain.Interfaces.Repositories.Properties;

public interface IPropertyScanRepository : IRepositoryBase<PropertyScan>
{
    Task<IEnumerable<PropertyScan>> IndexActiveAsync();
}

