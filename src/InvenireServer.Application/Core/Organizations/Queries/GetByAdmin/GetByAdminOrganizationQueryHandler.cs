using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Queries.GetByAdmin;

public class GetByAdminOrganizationQueryHandler : IRequestHandler<GetByAdminOrganizationQuery, OrganizationDto>
{
    private readonly IServiceManager _services;

    public GetByAdminOrganizationQueryHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task<OrganizationDto> Handle(GetByAdminOrganizationQuery request, CancellationToken ct)
        => await _services.Organizations.Dto.TryGetForAsync(await _services.Admins.GetAsync(request.Jwt)) ?? throw new BadRequest400Exception("You have not created an organization.");
}