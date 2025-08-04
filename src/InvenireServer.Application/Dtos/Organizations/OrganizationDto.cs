using System.Linq.Expressions;
using System.Text.Json.Serialization;
using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Dtos.Organizations;

public class OrganizationDto
{
    [JsonPropertyName("id")]
    public required Guid Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("last_updated_at")]
    public required DateTimeOffset? LastUpdatedAt { get; set; }

    [JsonPropertyName("admin")]
    public required AdminDto? Admin { get; set; }

    [JsonPropertyName("property")]
    public required PropertyDto? Property { get; set; }

    [JsonPropertyName("employees")]
    public required List<EmployeeDto> Employees { get; set; } = [];

    [JsonPropertyName("invitations")]
    public required List<OrganizationInvitationDto> Invitations { get; set; } = [];

    public static Expression<Func<Organization, OrganizationDto>> FromOrganizationSelector
    {
        get
        {
            return o => new OrganizationDto
            {
                Id = o.Id,
                Name = o.Name,
                CreatedAt = o.CreatedAt,
                LastUpdatedAt = o.LastUpdatedAt,
                Admin = o.Admin == null ? null : new AdminDto
                {
                    Id = o.Admin.Id,
                    OrganizationId = o.Admin.OrganizationId,
                    Name = o.Admin.Name,
                    EmailAddress = o.Admin.EmailAddress,
                    CreatedAt = o.Admin.CreatedAt,
                    LastUpdatedAt = o.Admin.LastUpdatedAt
                },
                Property = o.Property == null ? null : new PropertyDto
                {
                    Id = o.Property.Id,
                    OrganizationId = o.Property.OrganizationId,
                    CreatedAt = o.Property.CreatedAt,
                    LastUpdatedAt = o.LastUpdatedAt,
                    ItemsSummary = null,
                    ScansSummary = null,
                    SuggestionsSummary = null,
                },
                Employees = o.Employees.Count == 0 ? new() : o.Employees.Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    OrganizationId = e.OrganizationId,
                    Name = e.Name,
                    EmailAddress = e.EmailAddress,
                    CreatedAt = e.CreatedAt,
                    LastUpdatedAt = e.LastUpdatedAt,
                    Suggestions = new(),
                    AssignedItems = new(),
                }).ToList(),
                Invitations = o.Invitations.Count == 0 ? new() : o.Invitations.Select(i => new OrganizationInvitationDto
                {

                    Id = i.Id,
                    OrganizationId = i.OrganizationId,
                    Description = i.Description,
                    CreatedAt = i.CreatedAt,
                    LastUpdatedAt = i.LastUpdatedAt,
                    Employee = null,
                }).ToList()
            };
        }
    }
}
