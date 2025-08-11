using InvenireServer.Application.Core.Properties.Suggestions.Commands;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Create;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Tests.Extensions.Properties;

public static class PropertySuggestionTestExtensions
{
    public static CreatePropertySuggestionCommand ToCreatePropertySuggestionCommand(this PropertySuggestion suggestion, PropertySuggestionPayload commands)
    {
        var dto = new CreatePropertySuggestionCommand
        {
            Id = suggestion.Id,
            Name = suggestion.Name,
            Description = suggestion.Description,
            Payload = commands
        };

        return dto;
    }
}
