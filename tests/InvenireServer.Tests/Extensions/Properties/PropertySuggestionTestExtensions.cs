using InvenireServer.Application.Core.Properties.Suggestions.Commands;
using InvenireServer.Application.Core.Properties.Suggestions.Commands.Create;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Tests.Extensions.Properties;

/// <summary>
/// Provides test extensions for <see cref="PropertySuggestion"/>.
/// </summary>
public static class PropertySuggestionTestExtensions
{
    /// <summary>
    /// Creates a <see cref="CreatePropertySuggestionCommand"/> from a suggestion.
    /// </summary>
    /// <param name="suggestion">Source suggestion.</param>
    /// <param name="commands">Suggestion payload commands.</param>
    /// <returns>Create property suggestion command.</returns>
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
