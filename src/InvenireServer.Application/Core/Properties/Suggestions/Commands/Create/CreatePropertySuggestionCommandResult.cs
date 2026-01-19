using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Create;

/// <summary>
/// Represents the result of creating a property suggestion.
/// </summary>
public class CreatePropertySuggestionCommandResult
{
    public required PropertySuggestion Suggestion { get; set; }
}
