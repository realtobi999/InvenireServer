using InvenireServer.Domain.Entities.Common;
using InvenireServer.Application.Dtos.Organizations;

namespace InvenireServer.Application.Core.Organizations.Invitations.Queries.GetById;

public record GetByIdOrganizationInvitationQuery : IRequest<OrganizationInvitationDto>
{
    public required Jwt Jwt { get; set; }
    public required Guid InvitationId { get; set; }
}
