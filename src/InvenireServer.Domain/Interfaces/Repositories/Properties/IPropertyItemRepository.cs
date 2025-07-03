using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Domain.Interfaces.Repositories.Properties;

public interface IPropertyItemRepository : IRepositoryBase<PropertyItem>
{
    public Task<bool> IsInventoryNumberUniqueAsync(PropertyItem item, Property property);
    public Task<bool> IsRegistrationNumberUniqueAsync(PropertyItem item, Property property);
    public Task<bool> IsDocumentNumberUniqueAsync(PropertyItem item, Property property);
    public Task<bool> IsSerialNumberUniqueAsync(PropertyItem item, Property property);
}