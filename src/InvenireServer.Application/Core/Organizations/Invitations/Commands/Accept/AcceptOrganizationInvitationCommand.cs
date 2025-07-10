using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Accept;

public class AcceptOrganizationInvitationCommand : IRequest
{
    public required Jwt Jwt { get; init; }
    public required Guid OrganizationId { get; init; }
}