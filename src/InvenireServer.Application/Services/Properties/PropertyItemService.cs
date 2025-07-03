using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Interfaces.Services.Properties;

namespace InvenireServer.Application.Services.Properties;

public class PropertyItemService : IPropertyItemService
{
    private readonly IRepositoryManager _repositories;
    private readonly IValidator<PropertyItem> _validator;

    public PropertyItemService(IRepositoryManager repositories, IValidator<PropertyItem> validator)
    {
        _validator = validator;
        _repositories = repositories;
    }

    public async Task CreateAsync(PropertyItem item)
    {
        await CreateAsync([item]);
    }

    public async Task CreateAsync(IEnumerable<PropertyItem> items)
    {
        foreach (var item in items)
        {
            var (valid, exception) = await _validator.ValidateAsync(item);
            if (!valid && exception is not null) throw exception;

            _repositories.Properties.Items.Create(item);
        }

        await _repositories.SaveOrThrowAsync();
    }
}