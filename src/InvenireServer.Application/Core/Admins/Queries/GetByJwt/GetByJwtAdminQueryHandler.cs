using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Admins.Queries.GetByJwt;

public class GetByJwtAdminQueryHandler : IRequestHandler<GetByJwtAdminQuery, GetByJwtAdminQueryResponse>
{
    private readonly IServiceManager _services;

    public GetByJwtAdminQueryHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task<GetByJwtAdminQueryResponse> Handle(GetByJwtAdminQuery request, CancellationToken ct)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt);

        return new GetByJwtAdminQueryResponse
        {
            AdminDto = new AdminDto
            {
                Id = admin.Id,
                OrganizationId = admin.OrganizationId,
                Name = admin.Name,
                EmailAddress = admin.EmailAddress,
                CreatedAt = admin.CreatedAt,
                LastUpdatedAt = admin.LastUpdatedAt,
            },
        };
    }
}
