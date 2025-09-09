using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Invitations.Queries.GetById;

public class GetByIdOrganizationInvitationQueryHandler : IRequestHandler<GetByIdOrganizationInvitationQuery, OrganizationInvitationDto>
{
    private readonly IRepositoryManager _repositories;

    public GetByIdOrganizationInvitationQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<OrganizationInvitationDto> Handle(GetByIdOrganizationInvitationQuery request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");

        var invitation = await _repositories.Organizations.Invitations.GetAsync(new QueryOptions<OrganizationInvitation, OrganizationInvitationDto>
        {
            Selector = OrganizationInvitationDto.FromInvitationSelector,
            Filtering = new QueryFilteringOptions<OrganizationInvitation>
            {
                Filters =
                [
                    i => i.Id == request.InvitationId
                ]
            }
        }) ?? throw new NotFound404Exception("The invitation was not found in the system.");

        if (invitation.OrganizationId != organization.Id) throw new Unauthorized401Exception("The invitation is not from the admin's organization.");

        return invitation;
    }
}
