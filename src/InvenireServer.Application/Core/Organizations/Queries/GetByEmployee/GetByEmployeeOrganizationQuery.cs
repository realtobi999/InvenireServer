using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Queries.GetByEmployee;

/// <summary>
/// Represents a query to get an organization for an employee.
/// </summary>
public record GetByEmployeeOrganizationQuery : IRequest<OrganizationDto>
{
    public required Jwt Jwt { get; init; }
}
