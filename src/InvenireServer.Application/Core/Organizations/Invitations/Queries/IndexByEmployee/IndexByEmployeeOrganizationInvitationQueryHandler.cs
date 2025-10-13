using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;

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
            Selector = OrganizationInvitationDtoSelector,
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

    private static Expression<Func<OrganizationInvitation, OrganizationInvitationDto>> OrganizationInvitationDtoSelector
    {
        get
        {
            return i => new OrganizationInvitationDto
            {
                Id = i.Id,
                OrganizationId = i.OrganizationId,
                Description = i.Description,
                CreatedAt = i.CreatedAt,
                LastUpdatedAt = i.LastUpdatedAt,
                Employee = i.Employee == null ? null : new EmployeeDto
                {
                    Id = i.Employee.Id,
                    OrganizationId = i.Employee.OrganizationId,
                    FirstName = i.Employee.FirstName,
                    LastName = i.Employee.LastName,
                    FullName = $"{i.Employee.FirstName} {i.Employee.LastName}",
                    EmailAddress = i.Employee.EmailAddress,
                    CreatedAt = i.Employee.CreatedAt,
                    LastUpdatedAt = i.Employee.LastUpdatedAt,
                }
            };
        }
    }
}
