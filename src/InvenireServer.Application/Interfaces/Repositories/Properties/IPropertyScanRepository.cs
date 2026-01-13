using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Interfaces.Repositories.Properties;

/// <summary>
/// Defines a repository for property scans.
/// </summary>
public interface IPropertyScanRepository : IRepositoryBase<PropertyScan>
{
    /// <summary>
    /// Registers property items for the specified scan.
    /// </summary>
    /// <param name="scan">Scan to register items for.</param>
    /// <returns>Awaitable task representing the registration.</returns>
    Task RegisterItemsAsync(PropertyScan scan);

    /// <summary>
    /// Gets the in-progress scan for a property.
    /// </summary>
    /// <param name="property">Property to resolve the scan for.</param>
    /// <returns>Awaitable task returning the scan or null.</returns>
    Task<PropertyScan?> GetInProgressForAsync(Property property);

    /// <summary>
    /// Returns in-progress scans for a property.
    /// </summary>
    /// <param name="property">Property to resolve scans for.</param>
    /// <returns>Awaitable task returning the scans.</returns>
    Task<IEnumerable<PropertyScan>> IndexInProgressForAsync(Property property);
}
