using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Domain.Interfaces.Services.Properties;

public interface IPropertyItemService
{
    Task CreateAsync(PropertyItem item);
    Task CreateAsync(IEnumerable<PropertyItem> items);
}
