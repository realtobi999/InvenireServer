using InvenireServer.Domain.Entities.Common;
using InvenireServer.Application.Dtos.Organizations;

namespace InvenireServer.Application.Core.Organizations.Queries.GetByAdmin;

public record GetByAdminOrganizationQuery : IRequest<OrganizationDto>
{
    public required Jwt Jwt { get; init; }
}
