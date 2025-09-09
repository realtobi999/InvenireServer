using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Common.Queries;

namespace InvenireServer.Application.Core.Properties.Queries.GetByAdmin;

public class GetByAdminPropertyQueryHandler : IRequestHandler<GetByAdminPropertyQuery, PropertyDto>
{
    private readonly IRepositoryManager _repositories;

    public GetByAdminPropertyQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<PropertyDto> Handle(GetByAdminPropertyQuery request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");

        return await _repositories.Properties.GetAsync(new QueryOptions<Property, PropertyDto>
        {
            Selector = PropertyDto.FromPropertySelector,
            Filtering = new QueryFilteringOptions<Property>
            {
                Filters =
                [
                    p => p.OrganizationId == organization.Id,
                ]
            }
        }) ?? throw new BadRequest400Exception("The organization doesn't have a property.");
    }
}
