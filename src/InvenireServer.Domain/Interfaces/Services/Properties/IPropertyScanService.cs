using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Domain.Interfaces.Services.Properties;

public interface IPropertyScanService
{
    Task<IEnumerable<PropertyScan>> IndexInProgressAsync(Property property);
    Task<PropertyScan> GetAsync(Expression<Func<PropertyScan, bool>> predicate);
    Task<PropertyScan?> TryGetAsync(Expression<Func<PropertyScan, bool>> predicate);
    Task CreateAsync(PropertyScan scan);
    Task UpdateAsync(PropertyScan scan);
}
