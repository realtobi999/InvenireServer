using System.Linq.Expressions;
using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Application.Core.Properties.Suggestions.Commands;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Dtos.Properties;

[JsonResponse]
public record PropertySuggestionDto
{
    // Properties.

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

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("payload")]
    public PropertySuggestionPayload? Payload { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("payload_string")]
    public string? PayloadString { get; set; }

    [JsonPropertyName("status")]
    public required PropertySuggestionStatus Status { get; set; }

    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("resolved_at")]
    public required DateTimeOffset? ResolvedAt { get; set; }

    [JsonPropertyName("last_updated_at")]
    public required DateTimeOffset? LastUpdatedAt { get; set; }

    // Selectors.

    public static Expression<Func<PropertySuggestion, PropertySuggestionDto>> IndexByAdminSelector
    {
        get
        {
            return s => new PropertySuggestionDto
            {
                Id = s.Id,
                EmployeeId = s.EmployeeId,
                PropertyId = s.PropertyId,
                Name = s.Name,
                Description = s.Description,
                Feedback = s.Feedback,
                Payload = null,
                PayloadString = s.PayloadString,
                Status = s.Status,
                CreatedAt = s.CreatedAt,
                ResolvedAt = s.ResolvedAt,
                LastUpdatedAt = s.LastUpdatedAt,
            };
        }
    }
}

