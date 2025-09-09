using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Common.Queries;

namespace InvenireServer.Application.Core.Organizations.Invitations.Queries.IndexByEmployee;

public class IndexByEmployeeOrganizationInvitationQueryHandler : IRequestHandler<IndexByEmployeeOrganizationInvitationQuery, IndexByEmployeeOrganizationInvitationQueryResponse>
{
    private readonly IRepositoryManager _repositories;

    public IndexByEmployeeOrganizationInvitationQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<IndexByEmployeeOrganizationInvitationQueryResponse> Handle(IndexByEmployeeOrganizationInvitationQuery request, CancellationToken ct)
    {
        var employee = await _repositories.Employees.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The employee was not found in the system.");

        var query = new QueryOptions<OrganizationInvitation, OrganizationInvitationDto>
        {
            Selector = OrganizationInvitationDto.FromInvitationSelector,
            Filtering = new QueryFilteringOptions<OrganizationInvitation>
            {
                Filters =
                [
                    i => i.Employee!.Id == employee.Id
                ]
            },
            Pagination = request.Pagination,
        };

        return new IndexByEmployeeOrganizationInvitationQueryResponse
        {
            Data = [.. await _repositories.Organizations.Invitations.IndexAsync(query)],
            Limit = request.Pagination.Limit,
            Offset = request.Pagination.Offset,
            TotalCount = await _repositories.Organizations.Invitations.CountAsync(query.Filtering.Filters!)
        };
    }
}
