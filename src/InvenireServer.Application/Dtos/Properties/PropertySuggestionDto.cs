using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Dtos.Properties;

[JsonResponse]
public class PropertySuggestionDto
{
    [JsonPropertyName("id")]
    public required Guid Id { get; set; }

    [JsonPropertyName("employee_id")]
    public required Guid? EmployeeId { get; set; }

    [JsonPropertyName("property_id")]
    public required Guid? PropertyId { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public required string? Description { get; set; }

    [JsonPropertyName("feedback")]
    public required string? Feedback { get; set; }

    [JsonPropertyName("payload_string")]
    public required string PayloadString { get; set; }

    [JsonPropertyName("status")]
    public required PropertySuggestionStatus Status { get; set; }

    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("resolved_at")]
    public required DateTimeOffset? ResolvedAt { get; set; }

    [JsonPropertyName("last_updated_at")]
    public required DateTimeOffset? LastUpdatedAt { get; set; }
}

[JsonResponse]
public class PropertySuggestionsSummary
{
    [JsonPropertyName("total_suggestions")]
    public required int TotalSuggestions { get; set; }
}