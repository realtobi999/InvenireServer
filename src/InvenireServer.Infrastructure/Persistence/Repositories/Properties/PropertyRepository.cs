using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Repositories.Properties;
using InvenireServer.Domain.Entities.Properties;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Properties;

public class PropertyRepository : RepositoryBase<Property>, IPropertyRepository
{
    public PropertyRepository(InvenireServerContext context) : base(context)
    {
        Dto = new PropertyDtoRepository(context);
        Items = new PropertyItemRepository(context);
        Scans = new PropertyScanRepository(context);
        Suggestions = new PropertySuggestionRepository(context);
    }

    public IPropertyDtoRepository Dto { get; }
    public IPropertyItemRepository Items { get; }
    public IPropertyScanRepository Scans { get; }
    public IPropertySuggestionRepository Suggestions { get; }
}

public class PropertyDtoRepository : IPropertyDtoRepository
{
    private readonly InvenireServerContext _context;

    public PropertyDtoRepository(InvenireServerContext context)
    {
        _context = context;
    }

    public async Task<PropertyDto?> GetAsync(Expression<Func<Property, bool>> predicate)
    {
        return await _context.Set<Property>()
            .AsNoTracking()
            .Where(predicate)
            .Select(PropertyDto.FromPropertySelector)
            .FirstOrDefaultAsync();
    }
}