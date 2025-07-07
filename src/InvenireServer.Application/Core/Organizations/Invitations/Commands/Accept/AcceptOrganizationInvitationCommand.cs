using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Accept;

public class AcceptOrganizationInvitationCommand : IRequest
{
    public required Jwt Jwt { get; set; }
    public required Guid OrganizationId { get; set; }
}