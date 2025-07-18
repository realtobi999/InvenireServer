using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Update;

[JsonRequest]
public record UpdatePropertySuggestionCommand : IRequest
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("payload")]
    public required PropertySuggestionPayload? Payload { get; set; }

    [JsonIgnore]
    public Jwt? Jwt { get; set; }

    [JsonIgnore]
    public Guid? SuggestionId { get; set; }
}
