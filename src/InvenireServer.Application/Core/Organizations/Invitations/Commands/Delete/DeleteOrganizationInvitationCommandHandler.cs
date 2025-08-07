
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Delete;

public class DeleteOrganizationInvitationCommandHandler : IRequestHandler<DeleteOrganizationInvitationCommand>
{
    private readonly IRepositoryManager _repositories;

    public DeleteOrganizationInvitationCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task Handle(DeleteOrganizationInvitationCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var invitation = await _repositories.Organizations.Invitations.GetAsync(i => i.Id == request.Id) ?? throw new NotFound404Exception("The invitation was not found in the system.");

        if (invitation.OrganizationId != organization.Id) throw new Unauthorized401Exception("The invitation is not part of the organization.");
        organization.RemoveInvitation(invitation);

        _repositories.Organizations.Update(organization);
        _repositories.Organizations.Invitations.Delete(invitation);

        await _repositories.SaveOrThrowAsync();
    }
}
