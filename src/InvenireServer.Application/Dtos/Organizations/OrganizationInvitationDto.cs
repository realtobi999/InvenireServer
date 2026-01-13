using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Application.Dtos.Employees;

namespace InvenireServer.Application.Dtos.Organizations;

/// <summary>
/// Represents organization invitation data that is exposed to the client.
/// </summary>
[JsonResponse]
public class OrganizationInvitationDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("organization_id")]
    public Guid? OrganizationId { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("last_updated_at")]
    public DateTimeOffset? LastUpdatedAt { get; set; }

    [JsonPropertyName("employee")]
    public EmployeeDto? Employee { get; set; }
}
