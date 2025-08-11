using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Application.Interfaces.Repositories.Properties;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Properties;

public class PropertyRepository : RepositoryBase<Property>, IPropertyRepository
{
    public PropertyRepository(InvenireServerContext context) : base(context)
    {
        Items = new PropertyItemRepository(context);
        Scans = new PropertyScanRepository(context);
        Suggestions = new PropertySuggestionRepository(context);
    }

    public IPropertyItemRepository Items { get; }
    public IPropertyScanRepository Scans { get; }
    public IPropertySuggestionRepository Suggestions { get; }

    public async Task<Property?> GetForAsync(Organization organization)
    {
        return await GetAsync(p => p.OrganizationId == organization.Id);
    }
}
