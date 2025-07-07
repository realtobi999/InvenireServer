using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Domain.Interfaces.Services.Properties;

public interface IPropertyItemService
{
    Task<PropertyItem> GetAsync(Expression<Func<PropertyItem, bool>> predicate);
    Task CreateAsync(PropertyItem item);
    Task CreateAsync(IEnumerable<PropertyItem> items);
    Task UpdateAsync(PropertyItem item);
    Task UpdateAsync(IEnumerable<PropertyItem> items);
    Task DeleteAsync(PropertyItem item);
    Task DeleteAsync(IEnumerable<PropertyItem> items);
}