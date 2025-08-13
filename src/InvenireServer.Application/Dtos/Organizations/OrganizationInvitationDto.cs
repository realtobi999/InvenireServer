using System.Linq.Expressions;
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

    public static Expression<Func<OrganizationInvitation, OrganizationInvitationDto>> FromInvitationSelector
    {
        get
        {
            return i => new OrganizationInvitationDto
            {
                Id = i.Id,
                OrganizationId = i.OrganizationId,
                Description = i.Description,
                CreatedAt = i.CreatedAt,
                LastUpdatedAt = i.LastUpdatedAt,
                Employee = i.Employee == null ? null : new EmployeeDto
                {
                    Id = i.Employee.Id,
                    OrganizationId = i.Employee.OrganizationId,
                    FirstName = i.Employee.FirstName,
                    LastName = i.Employee.LastName,
                    FullName = $"{i.Employee.FirstName} {i.Employee.LastName}",
                    EmailAddress = i.Employee.EmailAddress,
                    CreatedAt = i.Employee.CreatedAt,
                    LastUpdatedAt = i.Employee.LastUpdatedAt,
                    AssignedItems = new(),
                    Suggestions = new()
                }
            };
        }
    }
}
