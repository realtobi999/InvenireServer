using System.Text.Json.Serialization;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Dtos.Properties;

public class PropertyDto
{
    [JsonPropertyName("id")]
    public required Guid Id { get; set; }

    [JsonPropertyName("organization_id")]
    public required Guid? OrganizationId { get; set; }

    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("last_updated_at")]
    public required DateTimeOffset? LastUpdatedAt { get; set; }

    [JsonPropertyName("items")]
    public required List<PropertyItemDto> Items { get; set; } = [];

    [JsonPropertyName("scans")]
    public required List<PropertyScanDto> Scans { get; set; } = [];

    [JsonPropertyName("suggestions")]
    public required List<PropertySuggestionDto> Suggestions { get; set; } = [];
}
