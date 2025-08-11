using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Application.Dtos.Properties;

namespace InvenireServer.Application.Core.Properties.Scans.Queries.IndexByAdmin;

[JsonResponse]
public class IndexByAdminPropertyScanQueryResponse
{
    [JsonPropertyName("data")]
    public required List<PropertyScanDto> Data { get; set; }

    [JsonPropertyName("limit")]
    public required int Limit { get; set; }

    [JsonPropertyName("offset")]
    public required int Offset { get; set; }

    [JsonPropertyName("total_count")]
    public required int TotalCount { get; set; }
}
