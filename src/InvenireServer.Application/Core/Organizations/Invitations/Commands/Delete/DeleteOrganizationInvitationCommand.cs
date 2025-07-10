using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Delete;

public class DeleteOrganizationInvitationCommand : IRequest
{
    public required Guid Id { get; init; }
    public required Jwt Jwt { get; set; }
}
