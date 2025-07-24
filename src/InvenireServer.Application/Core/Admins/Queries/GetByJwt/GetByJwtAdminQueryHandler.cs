using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Admins.Queries.GetByJwt;

public class GetByJwtAdminQueryHandler : IRequestHandler<GetByJwtAdminQuery, AdminDto>
{
    private readonly IServiceManager _services;

    public GetByJwtAdminQueryHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task<AdminDto> Handle(GetByJwtAdminQuery request, CancellationToken ct) => await _services.Admins.Dto.GetAsync(request.Jwt);
}
