using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Invitations.Queries.IndexByEmployee;

public record IndexByEmployeeOrganizationInvitationQuery : IRequest<IndexByEmployeeOrganizationInvitationQueryResponse>
{
    public required Jwt Jwt { get; init; }
    public required PaginationOptions Pagination { get; init; }
}
