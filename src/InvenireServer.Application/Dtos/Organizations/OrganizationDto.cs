using System.Linq.Expressions;
using System.Text.Json.Serialization;
using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Dtos.Organizations;

public class OrganizationDto
{
    // Properties.

    [JsonPropertyName("id")]
    public required Guid Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("last_updated_at")]
    public required DateTimeOffset? LastUpdatedAt { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("admin")]
    public AdminDto? Admin { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("property")]
    public PropertyDto? Property { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("employees")]
    public List<EmployeeDto>? Employees { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("invitations")]
    public List<OrganizationInvitationDto>? Invitations { get; set; }

    // Selectors.

    public static Expression<Func<Organization, OrganizationDto>> GetByIdQuerySelector
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
                    FirstName = o.Admin.FirstName,
                    LastName = o.Admin.LastName,
                    FullName = $"{o.Admin.FirstName} {o.Admin.LastName}",
                    EmailAddress = o.Admin.EmailAddress,
                    CreatedAt = o.Admin.CreatedAt,
                    LastUpdatedAt = o.Admin.LastUpdatedAt
                },
                Property = null,
                Employees = null,
                Invitations = null
            };
        }
    }

    public static Expression<Func<Organization, OrganizationDto>> GetByAdminQuerySelector
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
                    FirstName = o.Admin.FirstName,
                    LastName = o.Admin.LastName,
                    FullName = $"{o.Admin.FirstName} {o.Admin.LastName}",
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
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    FullName = $"{e.FirstName} {e.LastName}",
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
                    },
                }).ToList()
            };
        }
    }
}
