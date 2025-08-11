using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Interfaces.Repositories.Properties;

public interface IPropertyScanRepository : IRepositoryBase<PropertyScan>
{
    Task<PropertyScan?> GetInProgressForAsync(Property property);
    Task<IEnumerable<PropertyScan>> IndexInProgressForAsync(Property property);
}

