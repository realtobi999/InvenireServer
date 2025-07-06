using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Interfaces.Repositories.Properties;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Properties;

public class PropertyItemRepository : RepositoryBase<PropertyItem>, IPropertyItemRepository
{
    public PropertyItemRepository(InvenireServerContext context) : base(context)
    {
    }

    public async Task<bool> IsInventoryNumberUniqueAsync(PropertyItem item, Property property)
    {
        return !await Context.Set<PropertyItem>().AnyAsync(i => i.Id != item.Id && i.PropertyId == property.Id && i.InventoryNumber == item.InventoryNumber);
    }

    public async Task<bool> IsRegistrationNumberUniqueAsync(PropertyItem item, Property property)
    {
        return !await Context.Set<PropertyItem>().AnyAsync(i => i.Id != item.Id && i.PropertyId == property.Id && i.RegistrationNumber == item.RegistrationNumber);
    }

    public async Task<bool> IsDocumentNumberUniqueAsync(PropertyItem item, Property property)
    {
        return !await Context.Set<PropertyItem>().AnyAsync(i => i.Id != item.Id && i.PropertyId == property.Id && i.DocumentNumber == item.DocumentNumber);
    }

    public async Task<bool> IsSerialNumberUniqueAsync(PropertyItem item, Property property)
    {
        return !await Context.Set<PropertyItem>().AnyAsync(i => i.Id != item.Id && i.PropertyId == property.Id && i.SerialNumber == item.SerialNumber);
    }
}