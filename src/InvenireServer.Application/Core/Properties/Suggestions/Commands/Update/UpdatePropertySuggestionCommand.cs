using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Update;

/// <summary>
/// Represents a request to update a property suggestion.
/// </summary>
[JsonRequest]
public record UpdatePropertySuggestionCommand : IRequest
{
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("payload")]
    public required PropertySuggestionPayload Payload { get; init; }

    [JsonIgnore]
    public Jwt? Jwt { get; init; }

    [JsonIgnore]
    public Guid? SuggestionId { get; init; }
}
