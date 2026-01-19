using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Accept;

/// <summary>
/// Represents a request to accept an organization invitation.
/// </summary>
public record AcceptOrganizationInvitationCommand : IRequest
{
    public required Jwt Jwt { get; init; }
    public required Guid InvitationId { get; init; }
}