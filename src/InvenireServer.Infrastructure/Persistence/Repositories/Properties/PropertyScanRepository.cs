using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Interfaces.Repositories.Properties;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Properties;

/// <summary>
/// Default implementation of <see cref="IPropertyScanRepository"/>.
/// </summary>
public class PropertyScanRepository : RepositoryBase<PropertyScan>, IPropertyScanRepository
{
    public PropertyScanRepository(InvenireServerContext context) : base(context)
    {
    }

    /// <summary>
    /// Registers property items for the specified scan.
    /// </summary>
    /// <param name="scan">Scan to register items for.</param>
    /// <returns>Awaitable task representing the registration.</returns>
    public async Task RegisterItemsAsync(PropertyScan scan)
    {
        var items = await Context.Items.Where(i => i.PropertyId == scan.PropertyId).Select(i => new PropertyScanPropertyItem
        {
            Id = Guid.NewGuid(),
            IsScanned = false,
            CreatedAt = DateTimeOffset.UtcNow,
            ScannedAt = null,
            PropertyScanId = scan.Id,
            PropertyItemId = i.Id,
            PropertyItemEmployeeId = i.EmployeeId
        }).ToListAsync();

        // For providers that don't  support  bulk  operations  (e.g.  in-memory
        // databases), fall back to adding entities individually so the data  is
        // properly tracked and saved within the context. We do this so  we  can
        // run this method in tests.
        if (!Context.Database.IsRelational())
        {
            await Context.AddRangeAsync(items);
            await Context.SaveChangesAsync();
        }
        else
        {
            await Context.BulkInsertAsync(items);
        }
    }

    /// <summary>
    /// Gets the in-progress scan for a property.
    /// </summary>
    /// <param name="property">Property to resolve the scan for.</param>
    /// <returns>Awaitable task returning the scan or null.</returns>
    public async Task<PropertyScan?> GetInProgressForAsync(Property property)
    {
        return await GetAsync(s => s.PropertyId == property.Id && s.Status == PropertyScanStatus.IN_PROGRESS);
    }

    /// <summary>
    /// Returns in-progress scans for a property.
    /// </summary>
    /// <param name="property">Property to resolve scans for.</param>
    /// <returns>Awaitable task returning the scans.</returns>
    public async Task<IEnumerable<PropertyScan>> IndexInProgressForAsync(Property property)
    {
        return await IndexAsync(s => s.PropertyId == property.Id && s.Status == PropertyScanStatus.IN_PROGRESS);
    }

    /// <summary>
    /// Updates a property scan entity in the repository.
    /// </summary>
    /// <param name="scan">Property scan to update.</param>
    public override void Update(PropertyScan scan)
    {
        scan.LastUpdatedAt = DateTimeOffset.UtcNow;
        base.Update(scan);
    }
}
