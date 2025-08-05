using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Organizations.Invitations.Queries.GetByEmployee;

public class GetByEmployeeOrganizationInvitationQueryHandler : IRequestHandler<GetByEmployeeOrganizationInvitationQuery, IEnumerable<OrganizationInvitationDto>>
{
    private readonly IServiceManager _services;

    public GetByEmployeeOrganizationInvitationQueryHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task<IEnumerable<OrganizationInvitationDto>> Handle(GetByEmployeeOrganizationInvitationQuery request, CancellationToken ct)
        => await _services.Organizations.Invitations.Dto.IndexForAsync(await _services.Employees.GetAsync(request.Jwt), request.Pagination);
}
