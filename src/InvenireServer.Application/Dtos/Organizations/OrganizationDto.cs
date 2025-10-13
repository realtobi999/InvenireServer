using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Dtos.Properties;

namespace InvenireServer.Application.Dtos.Organizations;

[JsonResponse]
public class OrganizationDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("last_updated_at")]
    public DateTimeOffset? LastUpdatedAt { get; set; }

    [JsonPropertyName("admin")]
    public AdminDto? Admin { get; set; }

    [JsonPropertyName("property")]
    public PropertyDto? Property { get; set; }

    [JsonPropertyName("employees")]
    public List<EmployeeDto>? Employees { get; set; }

    [JsonPropertyName("invitations")]
    public List<OrganizationInvitationDto>? Invitations { get; set; }
}
