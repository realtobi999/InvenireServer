using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Interfaces.Repositories.Properties;

public interface IPropertyRepository : IRepositoryBase<Property>
{
    IPropertyDtoRepository Dto { get; }
    IPropertyItemRepository Items { get; }
    IPropertyScanRepository Scans { get; }
    IPropertySuggestionRepository Suggestions { get; }
}

public interface IPropertyDtoRepository
{
    Task<PropertyDto?> GetAsync(Expression<Func<Property, bool>> predicate);
}