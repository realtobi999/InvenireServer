using InvenireServer.Application.Core.Properties.Suggestions.Commands.Create;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Tests.Integration.Extensions.Properties;

public static class PropertySuggestionTestExtensions
{
    public static CreatePropertySuggestionCommand ToCreatePropertySuggestionCommand(this PropertySuggestion suggestion, CreatePropertySuggestionCommand.RequestBody body)
    {
        var dto = new CreatePropertySuggestionCommand
        {
            Id = suggestion.Id,
            Name = suggestion.Name,
            Description = suggestion.Description,
            Body = body
        };

        return dto;
    }
}
