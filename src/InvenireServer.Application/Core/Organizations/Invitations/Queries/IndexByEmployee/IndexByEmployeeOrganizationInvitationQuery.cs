using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Common.Queries;

namespace InvenireServer.Application.Core.Organizations.Invitations.Queries.IndexByEmployee;

/// <summary>
/// Represents a query to index organization invitations for an employee.
/// </summary>
public record IndexByEmployeeOrganizationInvitationQuery : IRequest<IndexByEmployeeOrganizationInvitationQueryResponse>
{
    public required Jwt Jwt { get; init; }
    public required QueryPaginationOptions Pagination { get; init; }
}
