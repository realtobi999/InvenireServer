using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Admins.Queries.GetByJwt;

public class GetByJwtAdminQueryHandler : IRequestHandler<GetByJwtAdminQuery, AdminDto>
{
    private readonly IRepositoryManager _services;

    public GetByJwtAdminQueryHandler(IRepositoryManager services)
    {
        _services = services;
    }

    public async Task<AdminDto> Handle(GetByJwtAdminQuery request, CancellationToken ct)
        => await _services.Admins.GetAsync(request.Jwt, new QueryOptions<Admin, AdminDto>
        {
            Selector = AdminDto.FromAdminSelector
        }) ?? throw new NotFound404Exception("The admin was not found in the system.");
}
