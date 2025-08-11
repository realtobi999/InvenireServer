using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Update;

public class UpdateOrganizationInvitationCommandHandler : IRequestHandler<UpdateOrganizationInvitationCommand>
{
    private readonly IRepositoryManager _repositories;

    public UpdateOrganizationInvitationCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task Handle(UpdateOrganizationInvitationCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var invitation = await _repositories.Organizations.Invitations.GetAsync(i => i.Id == request.InvitationId) ?? throw new NotFound404Exception("The invitation was not found in the system.");

        if (invitation.OrganizationId != organization.Id) throw new Unauthorized401Exception("The invitation isn't part of the organization.");

        invitation.Description = request.Description;

        _repositories.Organizations.Invitations.Update(invitation);

        await _repositories.SaveOrThrowAsync();
    }
}
