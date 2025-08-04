using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Repositories.Properties;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Properties;

public class PropertyItemRepository : RepositoryBase<PropertyItem>, IPropertyItemRepository
{
    public PropertyItemRepository(InvenireServerContext context) : base(context)
    {
        Dto = new PropertyItemDtoRepository(context);
    }

    public IPropertyItemDtoRepository Dto { get; }
}

public class PropertyItemDtoRepository : IPropertyItemDtoRepository
{
    private readonly InvenireServerContext _context;

    public PropertyItemDtoRepository(InvenireServerContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PropertyItemDto>> IndexAsync(Expression<Func<PropertyItem, bool>> predicate, PaginationParameters pagination)
    {
        return await _context.Set<PropertyItem>()
            .AsNoTracking()
            .Skip(pagination.Offset)
            .Take(pagination.Limit)
            .Where(predicate)
            .Select(PropertyItemDto.FromPropertyItemSelector)
            .ToListAsync();
    }
}