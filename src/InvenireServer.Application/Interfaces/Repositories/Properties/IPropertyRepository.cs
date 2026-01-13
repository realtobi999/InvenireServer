using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Organizations;
using System.Linq.Expressions;

namespace InvenireServer.Application.Interfaces.Repositories.Properties;

/// <summary>
/// Defines a repository for properties and related aggregates.
/// </summary>
public interface IPropertyRepository : IRepositoryBase<Property>
{
    /// <summary>
    /// Repository for property items.
    /// </summary>
    IPropertyItemRepository Items { get; }

    /// <summary>
    /// Repository for property scans.
    /// </summary>
    IPropertyScanRepository Scans { get; }

    /// <summary>
    /// Repository for property suggestions.
    /// </summary>
    IPropertySuggestionRepository Suggestions { get; }

    /// <summary>
    /// Gets the property for the specified organization.
    /// </summary>
    /// <param name="organization">Organization to resolve the property for.</param>
    /// <returns>Awaitable task returning the property or null.</returns>
    Task<Property?> GetForAsync(Organization organization);
}
