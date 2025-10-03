using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Queries.GetByEmployee;

public record GetByEmployeeOrganizationQuery : IRequest<OrganizationDto>
{
    public required Jwt Jwt { get; init; }
}
