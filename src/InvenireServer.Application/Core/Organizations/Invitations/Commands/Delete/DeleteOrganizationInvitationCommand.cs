using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Delete;

public record DeleteOrganizationInvitationCommand : IRequest
{
    public required Jwt Jwt { get; init; }
    public required Guid Id { get; init; }
}
