using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Interfaces.Services.Properties;

namespace InvenireServer.Application.Services.Properties;

public class PropertyService : IPropertyService
{
    private readonly IRepositoryManager _repositories;

    public PropertyService(IRepositoryManager repositories)
    {
        _repositories = repositories;

        Items = new PropertyItemService();
    }

    public IPropertyItemService Items { get; }

    public async Task CreateAsync(Property property)
    {
        _repositories.Properties.Create(property);
        await _repositories.SaveOrThrowAsync();
    }
}
