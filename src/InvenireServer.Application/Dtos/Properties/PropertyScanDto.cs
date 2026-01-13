using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Dtos.Properties;

/// <summary>
/// Represents property scan data that is exposed to the client.
/// </summary>
[JsonResponse]
public class PropertyScanDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("property_id")]
    public Guid? PropertyId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("status")]
    public PropertyScanStatus Status { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("completed_at")]
    public DateTimeOffset? CompletedAt { get; set; }

    [JsonPropertyName("last_updated_at")]
    public DateTimeOffset? LastUpdatedAt { get; init; }

    [JsonPropertyName("scanned_items_summary")]
    public PropertyScanDtoScannedItemsSummary? ScannedItemsSummary { get; set; }
}

/// <summary>
/// Represents property scan items summary data that is exposed to the client.
/// </summary>
[JsonResponse]
public class PropertyScanDtoScannedItemsSummary
{
    [JsonPropertyName("total_scanned_items")]
    public int TotalScannedItems { get; set; }

    [JsonPropertyName("total_items_to_scan")]
    public int TotalItemsToScan { get; set; }
}