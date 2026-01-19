using InvenireServer.Domain.Entities.Common;
using InvenireServer.Application.Dtos.Organizations;

namespace InvenireServer.Application.Core.Organizations.Queries.GetByAdmin;

/// <summary>
/// Represents a query to get an organization for an admin.
/// </summary>
public record GetByAdminOrganizationQuery : IRequest<OrganizationDto>
{
    public required Jwt Jwt { get; init; }
}
