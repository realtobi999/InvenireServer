using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Properties.Queries.GetByAdmin;

public class GetByAdminPropertyQueryHandler : IRequestHandler<GetByAdminPropertyQuery, PropertyDto>
{
    private readonly IServiceManager _services;

    public GetByAdminPropertyQueryHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task<PropertyDto> Handle(GetByAdminPropertyQuery request, CancellationToken ct)
        => await _services.Properties.Dto.TryGetForAsync(await _services.Admins.GetAsync(request.Jwt)) ?? throw new BadRequest400Exception("You have not created a property.");
}
