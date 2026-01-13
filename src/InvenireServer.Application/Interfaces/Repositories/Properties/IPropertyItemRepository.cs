using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Interfaces.Repositories.Properties;

/// <summary>
/// Defines a repository for property items.
/// </summary>
public interface IPropertyItemRepository : IRepositoryBase<PropertyItem>
{
    /// <summary>
    /// Marks the item as scanned in the specified scan.
    /// </summary>
    /// <param name="item">Item to mark as scanned.</param>
    /// <param name="scan">Scan to update.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    Task ScanAsync(PropertyItem item, PropertyScan scan);

    /// <summary>
    /// Builds a search predicate for property items.
    /// </summary>
    /// <param name="term">Search term to match against name, inventory number, and registration number.</param>
    /// <returns>Filter expression for searching items.</returns>
    Expression<Func<PropertyItem, bool>> BuildSearchExpression(string term);
}
