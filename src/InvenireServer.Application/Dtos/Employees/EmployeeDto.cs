using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Application.Dtos.Properties;

namespace InvenireServer.Application.Dtos.Employees;

/// <summary>
/// Represents employee data that is exposed to the client.
/// </summary>
[JsonResponse]
public record EmployeeDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("organization_id")]
    public Guid? OrganizationId { get; init; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; init; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; init; }

    [JsonPropertyName("full_name")]
    public string? FullName { get; init; }

    [JsonPropertyName("email_address")]
    public string? EmailAddress { get; init; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("last_updated_at")]
    public DateTimeOffset? LastUpdatedAt { get; init; }

    [JsonPropertyName("assigned_items")]
    public List<PropertyItemDto>? AssignedItems { get; set; }

    [JsonPropertyName("property_suggestions")]
    public List<PropertySuggestionDto>? Suggestions { get; set; }
}
