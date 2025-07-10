
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Delete;

public class DeleteOrganizationInvitationCommandHandler : IRequestHandler<DeleteOrganizationInvitationCommand>
{
    private readonly IServiceManager _services;

    public DeleteOrganizationInvitationCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(DeleteOrganizationInvitationCommand request, CancellationToken _)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt);
        var organization = await _services.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId) ?? throw new BadRequest400Exception("You have not created an organization. You must first create an organization before deleting any invitations.");
        var invitation = await _services.Organizations.Invitations.TryGetAsync(i => i.Id == request.Id) ?? throw new NotFound404Exception("The specified invitation does not exist or may have already been deleted.");

        organization.RemoveInvitation(invitation);

        await _services.Organizations.Invitations.DeleteAsync(invitation);
        await _services.Organizations.UpdateAsync(organization);
    }
}
