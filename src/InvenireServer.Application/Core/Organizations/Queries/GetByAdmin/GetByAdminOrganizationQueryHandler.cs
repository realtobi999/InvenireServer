using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common.Queries;

namespace InvenireServer.Application.Core.Organizations.Queries.GetByAdmin;

public class GetByAdminOrganizationQueryHandler : IRequestHandler<GetByAdminOrganizationQuery, OrganizationDto>
{
    private readonly IRepositoryManager _repositories;

    public GetByAdminOrganizationQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<OrganizationDto> Handle(GetByAdminOrganizationQuery request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system");

        return await _repositories.Organizations.GetAsync(new QueryOptions<Organization, OrganizationDto>()
        {
            Selector = OrganizationDto.GetByAdminQuerySelector,
            Filtering = new QueryFilteringOptions<Organization>
            {
                Filters =
                [
                    o => o.Id == admin.OrganizationId
                ]
            }
        }) ?? throw new NotFound404Exception("The admin doesn't own a organization");
    }
}