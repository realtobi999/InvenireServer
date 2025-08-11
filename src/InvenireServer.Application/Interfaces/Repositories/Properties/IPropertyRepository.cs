using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Organizations;
using System.Linq.Expressions;

namespace InvenireServer.Application.Interfaces.Repositories.Properties;

public interface IPropertyRepository : IRepositoryBase<Property>
{
    IPropertyItemRepository Items { get; }
    IPropertyScanRepository Scans { get; }
    IPropertySuggestionRepository Suggestions { get; }
    Task<Property?> GetForAsync(Organization organization);
}