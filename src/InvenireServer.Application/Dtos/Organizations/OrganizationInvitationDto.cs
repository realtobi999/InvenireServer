using System.Text.Json.Serialization;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Dtos.Organizations;

public class OrganizationInvitationDto
{
    [JsonPropertyName("id")]
    public required Guid Id { get; set; }

    [JsonPropertyName("organization_id")]
    public required Guid? OrganizationId { get; set; }

    [JsonPropertyName("description")]
    public required string? Description { get; set; }

    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("last_updated_at")]
    public required DateTimeOffset? LastUpdatedAt { get; set; }

    [JsonPropertyName("employee")]
    public required EmployeeDto? Employee { get; set; }
}
