using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Interfaces.Repositories.Properties;

public interface IPropertyScanRepository : IRepositoryBase<PropertyScan>
{
    Task<IEnumerable<PropertyScan>> IndexInProgressAsync(Property property);
}

