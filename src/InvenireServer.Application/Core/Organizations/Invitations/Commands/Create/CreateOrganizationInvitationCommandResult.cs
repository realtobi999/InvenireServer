using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;

public record CreateOrganizationInvitationCommandResult
{
    public required Organization Organization { get; init; }
    public required OrganizationInvitation Invitation { get; init; }
}