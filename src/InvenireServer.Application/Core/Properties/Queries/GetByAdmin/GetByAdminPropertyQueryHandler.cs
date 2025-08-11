using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;

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
        var property = await _repositories.Properties.GetAndProjectAsync(p => p.OrganizationId == organization.Id, PropertyDto.FromPropertySelector) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        return property;
    }
}
