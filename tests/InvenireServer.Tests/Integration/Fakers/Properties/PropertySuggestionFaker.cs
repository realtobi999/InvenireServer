using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Tests.Integration.Fakers.Properties;

public class PropertySuggestionFaker : Faker<PropertySuggestion>
{
    private PropertySuggestionFaker()
    {
        RuleFor(s => s.Id, f => Guid.NewGuid());
        RuleFor(s => s.Name, f => f.Lorem.Sentence(3));
        RuleFor(s => s.Description, f => f.Lorem.Paragraph());
        RuleFor(s => s.Feedback, f => f.Lorem.Sentence());
        RuleFor(s => s.Status, f => f.PickRandom<PropertySuggestionStatus>());
        RuleFor(s => s.CreatedAt, f => f.Date.PastOffset(5));
        RuleFor(s => s.LastUpdatedAt, f => f.Date.RecentOffset(30));
        RuleFor(s => s.RequestBody, f => f.Lorem.Paragraphs(1));
    }

    public static PropertySuggestion Fake()
    {
        return new PropertySuggestionFaker().Generate();
    }
}
