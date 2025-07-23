using System.Text.Json.Serialization;
using InvenireServer.Application.Dtos.Properties;

namespace InvenireServer.Application.Dtos.Employees;

public record EmployeeDto
{
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }

    [JsonPropertyName("organization_id")]
    public required Guid? OrganizationId { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("email_address")]
    public required string EmailAddress { get; init; }

    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("last_updated_at")]
    public required DateTimeOffset? LastUpdatedAt { get; init; }

    [JsonPropertyName("assigned_items")]
    public required List<PropertyItemDto> AssignedItems { get; set; } = [];

    [JsonPropertyName("property_suggestions")]
    public required List<PropertySuggestionDto> Suggestions { get; set; } = [];
}
