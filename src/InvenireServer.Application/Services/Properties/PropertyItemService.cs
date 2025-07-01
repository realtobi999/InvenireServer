using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Interfaces.Services.Properties;

namespace InvenireServer.Application.Services.Properties;

public class PropertyItemService : IPropertyItemService
{
    private readonly IRepositoryManager _repositories;

    public PropertyItemService(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task CreateAsync(PropertyItem item)
    {
        _repositories.Properties.Items.Create(item);
        await _repositories.SaveOrThrowAsync();
    }

    public async Task CreateAsync(IEnumerable<PropertyItem> items)
    {
        foreach (var item in items) _repositories.Properties.Items.Create(item);
        await _repositories.SaveOrThrowAsync();
    }
}
