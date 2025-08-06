using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Admins.Queries.GetByJwt;

public class GetByJwtAdminQueryHandler : IRequestHandler<GetByJwtAdminQuery, AdminDto>
{
    private readonly IRepositoryManager _services;

    public GetByJwtAdminQueryHandler(IRepositoryManager services)
    {
        _services = services;
    }

    public async Task<AdminDto> Handle(GetByJwtAdminQuery request, CancellationToken ct)
        => await _services.Admins.GetAndProjectToAsync(request.Jwt, AdminDto.FromAdminSelector) ?? throw new NotFound404Exception("The admin was not found in the system.");
}
