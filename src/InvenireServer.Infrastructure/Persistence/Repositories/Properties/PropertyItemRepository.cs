using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Interfaces.Repositories.Properties;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Properties;

/// <summary>
/// Default implementation of <see cref="IPropertyItemRepository"/>.
/// </summary>
public class PropertyItemRepository : RepositoryBase<PropertyItem>, IPropertyItemRepository
{
    public PropertyItemRepository(InvenireServerContext context) : base(context)
    {
    }

    /// <summary>
    /// Marks the item as scanned in the specified scan.
    /// </summary>
    /// <param name="item">Item to mark as scanned.</param>
    /// <param name="scan">Scan to update.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task ScanAsync(PropertyItem item, PropertyScan scan)
    {
        var field = await Context.ScansItems.FirstAsync(si => si.PropertyItemId == item.Id && si.PropertyScanId == scan.Id);

        field.ScannedAt = DateTimeOffset.UtcNow;
        field.IsScanned = true;
    }

    /// <summary>
    /// Builds a search predicate for property items.
    /// </summary>
    /// <param name="term">Search term to match against name, inventory number, and registration number.</param>
    /// <returns>Filter expression for searching items.</returns>
    public Expression<Func<PropertyItem, bool>> BuildSearchExpression(string term)
    {
        if (term.StartsWith('\"') && term.EndsWith('\"'))
        {
            term = term.Trim('"');
        }
        else
        {
            term = $"%{term}%";
        }

        return i =>
            EF.Functions.ILike(i.Name, term) ||
            EF.Functions.ILike(i.InventoryNumber, term) ||
            EF.Functions.ILike(i.RegistrationNumber, term);
    }

    /// <summary>
    /// Updates a property item entity in the repository.
    /// </summary>
    /// <param name="item">Property item to update.</param>
    public override void Update(PropertyItem item)
    {
        item.LastUpdatedAt = DateTimeOffset.UtcNow;
        base.Update(item);
    }
}
