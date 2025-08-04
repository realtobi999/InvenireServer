using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Organizations.Invitations.Queries.GetById;

public class GetByIdOrganizationInvitationQueryHandler : IRequestHandler<GetByIdOrganizationInvitationQuery, OrganizationInvitationDto>
{
    private readonly IServiceManager _services;

    public GetByIdOrganizationInvitationQueryHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task<OrganizationInvitationDto> Handle(GetByIdOrganizationInvitationQuery request, CancellationToken ct)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt);
        var invitation = await _services.Organizations.Invitations.Dto.GetAsync(i => i.Id == request.InvitationId && i.OrganizationId == admin.OrganizationId);

        return invitation;
    }
}
