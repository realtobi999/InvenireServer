using System.Linq.Expressions;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Interfaces.Services.Properties;

namespace InvenireServer.Application.Services.Properties;

public class PropertyItemService : IPropertyItemService
{
    private readonly IRepositoryManager _repositories;
    private readonly IEntityValidator<PropertyItem> _validator;

    public PropertyItemService(IRepositoryManager repositories, IEntityValidator<PropertyItem> validator)
    {
        _validator = validator;
        _repositories = repositories;
    }

    public async Task<PropertyItem> GetAsync(Expression<Func<PropertyItem, bool>> predicate)
    {
        var item = await _repositories.Properties.Items.GetAsync(predicate);

        if (item is null) throw new NotFound404Exception($"The requested {nameof(Property).ToLower()} was not found in the system.");

        return item;
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

    public async Task UpdateAsync(PropertyItem item)
    {
        await UpdateAsync([item]);
    }

    public async Task UpdateAsync(IEnumerable<PropertyItem> items)
    {
        foreach (var item in items)
        {
            item.LastUpdatedAt = DateTimeOffset.UtcNow;

            var (valid, exception) = await _validator.ValidateAsync(item);
            if (!valid && exception is not null) throw exception;

            _repositories.Properties.Items.Update(item);
        }

        await _repositories.SaveOrThrowAsync();
    }

    public async Task DeleteAsync(PropertyItem item)
    {
        await DeleteAsync([item]);
    }

    public async Task DeleteAsync(IEnumerable<PropertyItem> items)
    {
        foreach (var item in items) _repositories.Properties.Items.Delete(item);

        await _repositories.SaveOrThrowAsync();
    }
}