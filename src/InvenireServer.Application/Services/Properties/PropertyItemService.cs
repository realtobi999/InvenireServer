using FluentValidation;
using System.Linq.Expressions;
using FluentValidation.Results;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Validators.Properties.Items;
using InvenireServer.Application.Interfaces.Services.Properties;

namespace InvenireServer.Application.Services.Properties;

public class PropertyItemService : IPropertyItemService
{
    private readonly IRepositoryManager _repositories;

    public PropertyItemService(IRepositoryManager repositories)
    {
        _repositories = repositories;

        Dto = new PropertyItemDtoService(repositories);
    }

    public IPropertyItemDtoService Dto { get; }

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
            var result = new ValidationResult(PropertyItemEntityValidator.Validate(item));
            if (!result.IsValid) throw new ValidationException($"One or more core validation errors occurred for {nameof(PropertyItem).ToLower()} (ID: {item.Id}).", result.Errors);

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

            var result = new ValidationResult(PropertyItemEntityValidator.Validate(item));
            if (!result.IsValid) throw new ValidationException($"One or more core validation errors occurred for {nameof(PropertyItem).ToLower()} (ID: {item.Id}).", result.Errors);

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

public class PropertyItemDtoService : IPropertyItemDtoService
{
    private readonly IRepositoryManager _repositories;

    public PropertyItemDtoService(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<IEnumerable<PropertyItemDto>> IndexAsync(Expression<Func<PropertyItem, bool>> predicate, PaginationParameters pagination)
    {
        var items = await _repositories.Properties.Items.IndexAndProjectToAsync<PropertyItemDto>(predicate, PropertyItemDto.FromPropertyItemSelector, pagination);

        return items;
    }

    public async Task<IEnumerable<PropertyItemDto>> IndexForAsync(Property property, PaginationParameters pagination)
    {
        var items = await IndexAsync(i => i.PropertyId == property.Id, pagination);

        return items;
    }
}