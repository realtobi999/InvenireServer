using InvenireServer.Domain.Entities.Common;
using InvenireServer.Application.Dtos.Organizations;

namespace InvenireServer.Application.Core.Organizations.Invitations.Queries.GetById;

/// <summary>
/// Represents a query to get an organization invitation by ID.
/// </summary>
public record GetByIdOrganizationInvitationQuery : IRequest<OrganizationInvitationDto>
{
    public required Jwt Jwt { get; init; }
    public required Guid InvitationId { get; init; }
}
