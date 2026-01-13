using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Application.Core.Properties.Suggestions.Commands;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Dtos.Properties;

/// <summary>
/// Represents property suggestion data that is exposed to the client.
/// </summary>
[JsonResponse]
public record PropertySuggestionDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("employee_id")]
    public Guid? EmployeeId { get; set; }

    [JsonPropertyName("employee")]
    public EmployeeDto? Employee { get; set; }

    [JsonPropertyName("property_id")]
    public Guid? PropertyId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("feedback")]
    public string? Feedback { get; set; }


    [JsonPropertyName("payload")]
    public PropertySuggestionPayload? Payload { get; set; }
    [JsonPropertyName("payload_string")]
    public string? PayloadString { get; set; }

    [JsonPropertyName("status")]
    public PropertySuggestionStatus Status { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("resolved_at")]
    public DateTimeOffset? ResolvedAt { get; set; }

    [JsonPropertyName("last_updated_at")]
    public DateTimeOffset? LastUpdatedAt { get; set; }
}

