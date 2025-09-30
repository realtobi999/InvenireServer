using DocumentFormat.OpenXml.Office.CustomUI;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Tests.Fakers.Properties;

public class PropertyScanFaker : Faker<PropertyScan>
{
    private PropertyScanFaker()
    {
        RuleFor(s => s.Id, f => Guid.NewGuid());
        RuleFor(s => s.Name, f => f.Lorem.Sentence(3));
        RuleFor(s => s.Description, f => f.Lorem.Paragraph());
        RuleFor(s => s.Status, f => f.PickRandom<PropertyScanStatus>());
        RuleFor(s => s.CreatedAt, f => f.Date.PastOffset(5));
        RuleFor(s => s.CompletedAt, f => f.Date.FutureOffset(5));
        RuleFor(s => s.LastUpdatedAt, f => f.Date.RecentOffset(30));
    }

    public static PropertyScan Fake(IEnumerable<PropertyItem>? items = null)
    {
        var scan = new PropertyScanFaker().Generate();

        if (items is not null)
        {
            foreach (var item in items)
            {
                scan.ScannedItems.Add(new PropertyScanPropertyItem
                {
                    Id = Guid.NewGuid(),
                    IsScanned = false,
                    ScannedAt = null,
                    CreatedAt = DateTimeOffset.UtcNow,
                    PropertyItemId = item.Id,
                    PropertyScanId = scan.Id,
                });
            }
        }

        return scan;
    }
}
