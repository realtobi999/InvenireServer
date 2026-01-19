using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Tests.Fakers.Properties.Items;

/// <summary>
/// Provides fake <see cref="PropertyItemLocation"/> instances for tests.
/// </summary>
public class PropertyItemLocationFaker : Faker<PropertyItemLocation>
{
    private PropertyItemLocationFaker()
    {
        RuleFor(c => c.Room, f => f.Lorem.Sentence());
        RuleFor(c => c.Building, f => f.Lorem.Sentence());
        RuleFor(c => c.AdditionalNote, f => f.Lorem.Paragraph());
    }

    /// <summary>
    /// Creates a fake <see cref="PropertyItemLocation"/> instance.
    /// </summary>
    /// <returns>Fake <see cref="PropertyItemLocation"/> instance.</returns>
    public static PropertyItemLocation Fake()
    {
        return new PropertyItemLocationFaker().Generate();
    }
}
