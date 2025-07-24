using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Interfaces.Repositories.Properties;

public interface IPropertyRepository : IRepositoryBase<Property>
{
    IPropertyItemRepository Items { get; }
    IPropertyScanRepository Scans { get; }
    IPropertySuggestionRepository Suggestions { get; }
}