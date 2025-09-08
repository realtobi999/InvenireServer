using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;

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

        return await _repositories.Properties.GetAsync(p => p.OrganizationId == organization.Id, new QueryOptions<Property, PropertyDto>
        {
            Selector = PropertyDto.FromPropertySelector
        }) ?? throw new BadRequest400Exception("The organization doesn't have a property.");
    }
}
