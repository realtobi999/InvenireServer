using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Tests.Fakers.Properties;

/// <summary>
/// Provides fake <see cref="PropertySuggestion"/> instances for tests.
/// </summary>
public class PropertySuggestionFaker : Faker<PropertySuggestion>
{
    private PropertySuggestionFaker()
    {
        RuleFor(s => s.Id, f => Guid.NewGuid());
        RuleFor(s => s.Name, f => f.Lorem.Sentence(3));
        RuleFor(s => s.Description, f => f.Lorem.Paragraph());
        RuleFor(s => s.Feedback, f => f.Lorem.Sentence());
        RuleFor(s => s.PayloadString, string.Empty);
        RuleFor(s => s.Status, f => f.PickRandom<PropertySuggestionStatus>());
        RuleFor(s => s.CreatedAt, f => f.Date.PastOffset(5));
        RuleFor(s => s.ResolvedAt, f => f.Date.RecentOffset(30));
        RuleFor(s => s.LastUpdatedAt, f => f.Date.RecentOffset(30));
    }

    /// <summary>
    /// Creates a fake <see cref="PropertySuggestion"/> instance.
    /// </summary>
    /// <returns>Fake <see cref="PropertySuggestion"/> instance.</returns>
    public static PropertySuggestion Fake()
    {
        return new PropertySuggestionFaker().Generate();
    }
}
