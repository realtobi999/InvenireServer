using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

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
        var organization = await _repositories.Organizations.GetAndProjectAsync(o => o.Id == admin.OrganizationId, OrganizationDto.FromOrganizationSelector) ?? throw new NotFound404Exception("The admin doesn't own a organization");

        return organization;
    }

}