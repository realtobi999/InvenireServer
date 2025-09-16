using System.Linq.Expressions;
using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Dtos.Properties;

[JsonResponse]
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

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("items_summary")]
    public PropertyDtoItemsSummary? ItemsSummary { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("scans_summary")]
    public PropertyDtoScansSummary? ScansSummary { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("suggestions_summary")]
    public PropertyDtoSuggestionsSummary? SuggestionsSummary { get; set; }

    public static Expression<Func<Property, PropertyDto>> FromPropertySelector
    {
        get
        {
            return p => new PropertyDto
            {
                Id = p.Id,
                OrganizationId = p.OrganizationId,
                CreatedAt = p.CreatedAt,
                LastUpdatedAt = p.LastUpdatedAt,
                ItemsSummary = p.Items.Count == 0 ? null : new PropertyDtoItemsSummary
                {
                    TotalItems = p.Items.Count,
                    TotalValue = p.Items.Sum(i => i.Price),
                    AverageAge = p.Items.Average(i => (DateTime.Now - i.DateOfPurchase).TotalDays / 365.25),
                    AveragePrice = p.Items.Average(i => i.Price),
                },
                ScansSummary = p.Scans.Count == 0 ? null : new PropertyDtoScansSummary
                {
                    TotalScans = p.Scans.Count,
                },
                SuggestionsSummary = p.Suggestions.Count == 0 ? null : new PropertyDtoSuggestionsSummary
                {
                    TotalSuggestions = p.Suggestions.Count,
                    TotalApprovedSuggestions = p.Suggestions.Count(s => s.Status == PropertySuggestionStatus.APPROVED),
                    TotalPendingSuggestions = p.Suggestions.Count(s => s.Status == PropertySuggestionStatus.PENDING),
                    TotalDeclinedSuggestions = p.Suggestions.Count(s => s.Status == PropertySuggestionStatus.DECLINED)
                },
            };
        }
    }
}

[JsonResponse]
public record PropertyDtoItemsSummary
{
    [JsonPropertyName("total_items")]
    public required int TotalItems { get; set; }

    [JsonPropertyName("total_value")]
    public required double TotalValue { get; set; }

    [JsonPropertyName("average_price")]
    public required double AveragePrice { get; set; }

    [JsonPropertyName("average_age")]
    public required double AverageAge { get; set; }
}

[JsonResponse]
public record PropertyDtoScansSummary
{
    [JsonPropertyName("total_scans")]
    public required int TotalScans { get; set; }
}

[JsonResponse]
public class PropertyDtoSuggestionsSummary
{
    [JsonPropertyName("total_suggestions")]
    public required int TotalSuggestions { get; set; }

    [JsonPropertyName("total_approved_suggestions")]
    public required int TotalApprovedSuggestions { get; set; }

    [JsonPropertyName("total_pending_suggestions")]
    public required int TotalPendingSuggestions { get; set; }

    [JsonPropertyName("total_declined_suggestions")]
    public required int TotalDeclinedSuggestions { get; set; }
}