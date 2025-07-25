using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Invitations.Queries.GetByEmployee;

public record GetByEmployeeOrganizationInvitationQuery : IRequest<IEnumerable<OrganizationInvitationDto>>
{
    public required Jwt Jwt { get; set; }
}
