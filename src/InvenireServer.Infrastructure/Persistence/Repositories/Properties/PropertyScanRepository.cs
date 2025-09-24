using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Interfaces.Repositories.Properties;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Properties;

public class PropertyScanRepository : RepositoryBase<PropertyScan>, IPropertyScanRepository
{
    public PropertyScanRepository(InvenireServerContext context) : base(context)
    {
    }

    public async Task RegisterItemsAsync(PropertyScan scan)
    {
        var items = Context.Items.Where(i => i.PropertyId == scan.PropertyId).Select(i => new PropertyScanPropertyItem
        {
            IsScanned = false,
            PropertyItemId = i.Id,
            PropertyScanId = scan.Id
        });

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

    public async Task<PropertyScan?> GetInProgressForAsync(Property property)
    {
        return await GetAsync(s => s.PropertyId == property.Id && s.Status == PropertyScanStatus.IN_PROGRESS);
    }

    public async Task<IEnumerable<PropertyScan>> IndexInProgressForAsync(Property property)
    {
        return await IndexAsync(s => s.PropertyId == property.Id && s.Status == PropertyScanStatus.IN_PROGRESS);
    }

    public override void Update(PropertyScan scan)
    {
        scan.LastUpdatedAt = DateTimeOffset.UtcNow;
        base.Update(scan);
    }
}
