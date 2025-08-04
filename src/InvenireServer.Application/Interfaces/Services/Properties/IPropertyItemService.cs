using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Interfaces.Services.Properties;

public interface IPropertyItemService
{
    IPropertyItemDtoService Dto { get; }
    Task<PropertyItem> GetAsync(Expression<Func<PropertyItem, bool>> predicate);
    Task CreateAsync(PropertyItem item);
    Task CreateAsync(IEnumerable<PropertyItem> items);
    Task UpdateAsync(PropertyItem item);
    Task UpdateAsync(IEnumerable<PropertyItem> items);
    Task DeleteAsync(PropertyItem item);
    Task DeleteAsync(IEnumerable<PropertyItem> items);
}

public interface IPropertyItemDtoService
{
    Task<IEnumerable<PropertyItemDto>> IndexAsync(Expression<Func<PropertyItem, bool>> predicate, PaginationParameters pagination);
    Task<IEnumerable<PropertyItemDto>> IndexForAsync(Property property, PaginationParameters pagination);
}