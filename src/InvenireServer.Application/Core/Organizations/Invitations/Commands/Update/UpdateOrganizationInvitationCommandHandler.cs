
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Update;

public class UpdateOrganizationInvitationCommandHandler : IRequestHandler<UpdateOrganizationInvitationCommand>
{
    private readonly IServiceManager _services;

    public UpdateOrganizationInvitationCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(UpdateOrganizationInvitationCommand request, CancellationToken _)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt!);
        var invitation = await _services.Organizations.Invitations.GetAsync(i => i.Id == request.InvitationId);
        var organization = await _services.Organizations.TryGetForAsync(admin) ?? throw new BadRequest400Exception("You have not created an organization.");

        if (invitation.OrganizationId != organization.Id) throw new Unauthorized401Exception();

        invitation.Description = request.Description;

        await _services.Organizations.Invitations.UpdateAsync(invitation);
    }
}
