using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Domain.Interfaces.Repositories.Properties;

public interface IPropertyRepository : IRepositoryBase<Property>
{
    IPropertyItemRepository Items { get; }
    IPropertySuggestionRepository Suggestions { get; }
}