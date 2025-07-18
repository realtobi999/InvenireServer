using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Tests.Integration.Fakers.Properties.Items;

public class PropertyItemLocationFaker : Faker<PropertyItemLocation>
{
    private PropertyItemLocationFaker()
    {
        RuleFor(c => c.Room, f => f.Lorem.Sentence());
        RuleFor(c => c.Building, f => f.Lorem.Sentence());
        RuleFor(c => c.AdditionalNote, f => f.Lorem.Paragraph());
    }

    public static PropertyItemLocation Fake()
    {
        return new PropertyItemLocationFaker().Generate();
    }
}
