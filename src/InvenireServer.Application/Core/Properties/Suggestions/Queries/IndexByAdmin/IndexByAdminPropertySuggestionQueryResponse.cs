using System.Text.Json.Serialization;
using InvenireServer.Application.Dtos.Properties;

namespace InvenireServer.Application.Core.Properties.Suggestions.Queries.IndexByAdmin;

public record IndexByAdminPropertySuggestionQueryResponse
{
    [JsonPropertyName("data")]
    public required List<PropertySuggestionDto> Data { get; init; }

    [JsonPropertyName("limit")]
    public required int Limit { get; init; }

    [JsonPropertyName("offset")]
    public required int Offset { get; init; }

    [JsonPropertyName("total_count")]
    public required int TotalCount { get; init; }
}
