using System.Text.Json.Serialization;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Dtos.Properties;

public class PropertyScanDto
{
    [JsonPropertyName("id")]
    public required Guid Id { get; set; }

    [JsonPropertyName("property_id")]
    public required Guid PropertyId { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public required string? Description { get; set; }

    [JsonPropertyName("status")]
    public required PropertyScanStatus Status { get; set; }

    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("completed_at")]
    public required DateTimeOffset? CompletedAt { get; set; }

    [JsonPropertyName("last_updated_at")]
    public required DateTimeOffset LastUpdatedAt { get; init; }

    [JsonPropertyName("scanned_items")]
    public required List<PropertyItemDto> ScannedItems { get; set; } = [];
}
