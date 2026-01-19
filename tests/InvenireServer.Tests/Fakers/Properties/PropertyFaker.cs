using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Tests.Fakers.Properties;

/// <summary>
/// Provides fake <see cref="Property"/> instances for tests.
/// </summary>
public sealed class PropertyFaker : Faker<Property>
{
    private PropertyFaker()
    {
        RuleFor(p => p.Id, f => f.Random.Guid());
        RuleFor(p => p.CreatedAt, f => f.Date.PastOffset(5));
        RuleFor(p => p.LastUpdatedAt, f => f.Date.RecentOffset(30));
    }

    /// <summary>
    /// Creates a fake <see cref="Property"/> instance and assigns related entities.
    /// </summary>
    /// <param name="items">Items to assign to the property.</param>
    /// <param name="scans">Scans to assign to the property.</param>
    /// <param name="suggestions">Suggestions to assign to the property.</param>
    /// <returns>Fake <see cref="Property"/> instance.</returns>
    public static Property Fake(IEnumerable<PropertyItem>? items = null, IEnumerable<PropertyScan>? scans = null, IEnumerable<PropertySuggestion>? suggestions = null)
    {
        var property = new PropertyFaker().Generate();

        if (items is not null)
        {
            foreach (var i in items)
            {
                property.Items.Add(i);
                i.PropertyId = property.Id;
            }
        }
        if (scans is not null)
        {
            foreach (var s in scans)
            {
                property.Scans.Add(s);
                s.PropertyId = property.Id;
            }
        }
        if (suggestions is not null)
        {
            foreach (var s in suggestions)
            {
                property.Suggestions.Add(s);
                s.PropertyId = property.Id;
            }
        }

        return property;
    }
}
