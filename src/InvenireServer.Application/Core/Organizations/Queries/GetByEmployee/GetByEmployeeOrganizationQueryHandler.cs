using System.Linq.Expressions;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Organizations.Queries.GetByEmployee;

public class GetByEmployeeOrganizationQueryHandler : IRequestHandler<GetByEmployeeOrganizationQuery, OrganizationDto>
{
    private readonly IRepositoryManager _repositories;

    public GetByEmployeeOrganizationQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<OrganizationDto> Handle(GetByEmployeeOrganizationQuery request, CancellationToken ct)
    {
        var employee = await _repositories.Employees.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The employee was not found in the system");

        return await _repositories.Organizations.GetAsync(new QueryOptions<Organization, OrganizationDto>()
        {
            Selector = Selector,
            Filtering = new QueryFilteringOptions<Organization>
            {
                Filters =
                [
                    o => o.Id == employee.OrganizationId
                ]
            }
        }) ?? throw new BadRequest400Exception("The employee isn't part of any organization.");
    }

    private static Expression<Func<Organization, OrganizationDto>> Selector
    {
        get
        {
            return o => new OrganizationDto
            {
                Id = o.Id,
                Name = o.Name,
                CreatedAt = o.CreatedAt,
                LastUpdatedAt = o.LastUpdatedAt,
                Admin = o.Admin == null ? null : new AdminDto
                {
                    Id = o.Admin.Id,
                    OrganizationId = o.Admin.OrganizationId,
                    FirstName = o.Admin.FirstName,
                    LastName = o.Admin.LastName,
                    FullName = $"{o.Admin.FirstName} {o.Admin.LastName}",
                    EmailAddress = o.Admin.EmailAddress,
                    CreatedAt = o.Admin.CreatedAt,
                    LastUpdatedAt = o.Admin.LastUpdatedAt
                },
                Property = null,
                Employees = null,
                Invitations = null
            };
        }
    }
}
