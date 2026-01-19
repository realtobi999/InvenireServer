using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Application.Dtos.Properties;

namespace InvenireServer.Application.Core.Properties.Items.Queries.IndexByScan;

/// <summary>
/// Represents the response for indexing property items for a scan.
/// </summary>
[JsonResponse]
public record IndexByScanPropertyItemQueryResponse
{
    [JsonPropertyName("data")]
    public required List<PropertyItemDto> Data { get; set; }

    [JsonPropertyName("limit")]
    public required int Limit { get; set; }

    [JsonPropertyName("offset")]
    public required int Offset { get; set; }

    [JsonPropertyName("total_count")]
    public required int TotalCount { get; set; }
}
