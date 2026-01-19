using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Create;

/// <summary>
/// Represents a request to create a property suggestion.
/// </summary>
[JsonRequest]
public record CreatePropertySuggestionCommand : IRequest<CreatePropertySuggestionCommandResult>
{
    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("payload")]
    public required PropertySuggestionPayload Payload { get; init; }

    [JsonIgnore]
    public Jwt? Jwt { get; init; }
}
