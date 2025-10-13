using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;

namespace InvenireServer.Application.Dtos.Properties;

[JsonResponse]
public class PropertyDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("organization_id")]
    public Guid? OrganizationId { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("last_updated_at")]
    public DateTimeOffset? LastUpdatedAt { get; set; }

    [JsonPropertyName("items_summary")]
    public PropertyDtoItemsSummary? ItemsSummary { get; set; }

    [JsonPropertyName("scans_summary")]
    public PropertyDtoScansSummary? ScansSummary { get; set; }

    [JsonPropertyName("suggestions_summary")]
    public PropertyDtoSuggestionsSummary? SuggestionsSummary { get; set; }
}

[JsonResponse]
public record PropertyDtoItemsSummary
{
    [JsonPropertyName("total_items")]
    public int? TotalItems { get; set; }

    [JsonPropertyName("total_value")]
    public double? TotalValue { get; set; }

    [JsonPropertyName("average_price")]
    public double? AveragePrice { get; set; }

    [JsonPropertyName("average_age")]
    public double? AverageAge { get; set; }
}

[JsonResponse]
public record PropertyDtoScansSummary
{
    [JsonPropertyName("total_scans")]
    public int TotalScans { get; set; }

    [JsonPropertyName("total_active_scans")]
    public int TotalActiveScans { get; set; }

    [JsonPropertyName("last_active_scan")]
    public DateTimeOffset? LastActiveScan { get; set; }
}

[JsonResponse]
public record PropertyDtoSuggestionsSummary
{
    [JsonPropertyName("total_suggestions")]
    public int? TotalSuggestions { get; set; }

    [JsonPropertyName("total_approved_suggestions")]
    public int? TotalApprovedSuggestions { get; set; }

    [JsonPropertyName("total_pending_suggestions")]
    public int? TotalPendingSuggestions { get; set; }

    [JsonPropertyName("total_declined_suggestions")]
    public int? TotalDeclinedSuggestions { get; set; }
}