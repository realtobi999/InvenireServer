using System.Linq.Expressions;
using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Dtos.Properties;

[JsonResponse]
public class PropertyScanDto
{
    // Properties.

    [JsonPropertyName("id")]
    public required Guid Id { get; set; }

    [JsonPropertyName("property_id")]
    public required Guid? PropertyId { get; set; }

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
    public required DateTimeOffset? LastUpdatedAt { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("scanned_items_summary")]
    public PropertyScanDtoScannedItemsSummary? ScannedItemsSummary { get; set; }

    // Selectors.

    public static Expression<Func<PropertyScan, PropertyScanDto>> IndexByAdminSelector
    {
        get
        {
            return s => new PropertyScanDto
            {
                Id = s.Id,
                PropertyId = s.PropertyId,
                Name = s.Name,
                Description = s.Description,
                Status = s.Status,
                CreatedAt = s.CreatedAt,
                CompletedAt = s.CompletedAt,
                LastUpdatedAt = s.LastUpdatedAt,
                ScannedItemsSummary = s.ScannedItems.Count == 0 ? null : new PropertyScanDtoScannedItemsSummary
                {
                    TotalItemsToScan = s.ScannedItems.Count,
                    TotalScannedItems = s.ScannedItems.Where(si => si.IsScanned).ToList().Count,
                },
            };
        }
    }
}

[JsonResponse]
public class PropertyScanDtoScannedItemsSummary
{
    [JsonPropertyName("total_scanned_items")]
    public required int TotalScannedItems { get; set; }

    [JsonPropertyName("total_items_to_scan")]
    public required int TotalItemsToScan { get; set; }
}