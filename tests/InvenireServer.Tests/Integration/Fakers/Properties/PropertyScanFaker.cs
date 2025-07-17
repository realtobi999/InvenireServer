using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Tests.Integration.Fakers.Properties;

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
            foreach (var i in items)
            {
                scan.ScannedItems.Add(i);
            }
        }

        return scan;
    }
}
