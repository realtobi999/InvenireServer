using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Tests.Integration.Fakers.Properties;

public class PropertyFaker : Faker<Property>
{
    private PropertyFaker()
    {
        RuleFor(p => p.Id, f => f.Random.Guid());
        RuleFor(p => p.CreatedAt, f => f.Date.PastOffset(5));
        RuleFor(p => p.LastUpdatedAt, f => f.Date.RecentOffset(30));
    }

    public static Property Fake(IEnumerable<PropertyItem>? items = null)
    {
        var property = new PropertyFaker().Generate();

        if (items is not null) property.AddItems(items);

        return property;
    }
}