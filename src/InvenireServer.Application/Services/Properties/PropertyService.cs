using System.Linq.Expressions;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Interfaces.Services.Properties;

namespace InvenireServer.Application.Services.Properties;

public class PropertyService : IPropertyService
{
    private readonly IRepositoryManager _repositories;

    public PropertyService(IRepositoryManager repositories)
    {
        _repositories = repositories;

        Items = new PropertyItemService(repositories);
    }

    public IPropertyItemService Items { get; }

    public async Task<Property> GetAsync(Expression<Func<Property, bool>> predicate)
    {
        var property = await _repositories.Properties.GetAsync(predicate);

        if (property is null) throw new NotFound404Exception($"The requested {nameof(Property).ToLower()} was not found in the system.");

        return property;
    }

    public async Task CreateAsync(Property property)
    {
        _repositories.Properties.Create(property);
        await _repositories.SaveOrThrowAsync();
    }
}
