using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Domain.Interfaces.Services.Properties;

public interface IPropertyService
{
    IPropertyItemService Items { get; }
    Task CreateAsync(Property property);
}
