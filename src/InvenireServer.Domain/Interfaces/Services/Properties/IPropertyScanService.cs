using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Domain.Interfaces.Services.Properties;

public interface IPropertyScanService
{
    Task<IEnumerable<PropertyScan>> IndexActiveAsync();
    Task<PropertyScan> GetAsync(Expression<Func<PropertyScan, bool>> predicate);
    Task CreateAsync(PropertyScan scan);
    Task UpdateAsync(PropertyScan scan);
}
