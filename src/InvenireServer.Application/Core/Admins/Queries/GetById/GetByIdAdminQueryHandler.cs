
using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Admins.Queries.GetById;

public class GetByIdAdminQueryHandler : IRequestHandler<GetByIdAdminQuery, GetByIdAdminQueryResponse>
{
    private readonly IServiceManager _services;

    public GetByIdAdminQueryHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task<GetByIdAdminQueryResponse> Handle(GetByIdAdminQuery request, CancellationToken ct)
    {
        var admin = await _services.Admins.GetAsync(a => a.Id == request.AdminId);

        return new GetByIdAdminQueryResponse
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
