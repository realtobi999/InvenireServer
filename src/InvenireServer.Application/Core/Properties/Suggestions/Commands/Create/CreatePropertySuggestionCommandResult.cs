using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Create;

public class CreatePropertySuggestionCommandResult
{
    public required PropertySuggestion Suggestion { get; set; }
}
