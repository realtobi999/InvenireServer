using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;

/// <summary>
/// Represents the result of creating an organization invitation.
/// </summary>
public record CreateOrganizationInvitationCommandResult
{
    public required OrganizationInvitation Invitation { get; init; }
}