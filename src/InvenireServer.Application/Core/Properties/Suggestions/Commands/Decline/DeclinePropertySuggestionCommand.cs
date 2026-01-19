using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Decline;

/// <summary>
/// Represents a request to decline a property suggestion.
/// </summary>
[JsonRequest]
public record DeclinePropertySuggestionCommand : IRequest
{
    [JsonPropertyName("feedback")]
    public string? Feedback { get; init; }

    [JsonIgnore]
    public Jwt? Jwt { get; init; }

    [JsonIgnore]
    public Guid? SuggestionId { get; init; }
}
