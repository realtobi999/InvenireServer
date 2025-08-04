using System.Linq.Expressions;
using System.Text.Json.Serialization;
using InvenireServer.Domain.Entities.Properties;

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

    [JsonPropertyName("items_summary")]
    public required PropertyItemsSummary? ItemsSummary { get; set; }

    [JsonPropertyName("scans_summary")]
    public required PropertyScansSummary? ScansSummary { get; set; }

    [JsonPropertyName("suggestions_summary")]
    public required PropertySuggestionsSummary? SuggestionsSummary { get; set; }

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
                ItemsSummary = p.Items.Count == 0 ? null : new PropertyItemsSummary
                {
                    TotalItems = p.Items.Count,
                    TotalValue = p.Items.Sum(i => i.Price),
                },
                ScansSummary = p.Scans.Count == 0 ? null : new PropertyScansSummary
                {
                    TotalScans = p.Scans.Count,
                },
                SuggestionsSummary = p.Suggestions.Count == 0 ? null : new PropertySuggestionsSummary
                {
                    TotalSuggestions = p.Suggestions.Count,
                },
            };
        }
    }
}
