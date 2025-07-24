
using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Admins.Queries.GetById;

public class GetByIdAdminQueryHandler : IRequestHandler<GetByIdAdminQuery, AdminDto>
{
    private readonly IServiceManager _services;

    public GetByIdAdminQueryHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task<AdminDto> Handle(GetByIdAdminQuery request, CancellationToken ct) => await _services.Admins.Dto.GetAsync(a => a.Id == request.AdminId);
}
