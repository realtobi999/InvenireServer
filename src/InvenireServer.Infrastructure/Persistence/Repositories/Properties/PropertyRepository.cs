using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Application.Interfaces.Repositories.Properties;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Properties;

/// <summary>
/// Default implementation of <see cref="IPropertyRepository"/>.
/// </summary>
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

    /// <summary>
    /// Gets the property for the specified organization.
    /// </summary>
    /// <param name="organization">Organization to resolve the property for.</param>
    /// <returns>Awaitable task returning the property or null.</returns>
    public async Task<Property?> GetForAsync(Organization organization)
    {
        return await GetAsync(p => p.OrganizationId == organization.Id);
    }

    /// <summary>
    /// Updates a property entity in the repository.
    /// </summary>
    /// <param name="property">Property to update.</param>
    public override void Update(Property property)
    {
        property.LastUpdatedAt = DateTimeOffset.UtcNow;
        base.Update(property);
    }
}
