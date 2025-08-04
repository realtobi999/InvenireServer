using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Interfaces.Repositories.Properties;

public interface IPropertyItemRepository : IRepositoryBase<PropertyItem>
{
    IPropertyItemDtoRepository Dto { get; }
}

public interface IPropertyItemDtoRepository
{
    Task<IEnumerable<PropertyItemDto>> IndexAsync(Expression<Func<PropertyItem, bool>> predicate, PaginationParameters pagination);
}