using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Create;

[JsonRequest]
public record CreatePropertySuggestionCommand : IRequest<CreatePropertySuggestionCommandResult>
{
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("payload")]
    public required PropertySuggestionPayload Payload { get; set; }

    [JsonIgnore]
    public Jwt? Jwt { get; set; }
}
