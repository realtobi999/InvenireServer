using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Accept;

public class AcceptOrganizationInvitationCommandHandler : IRequestHandler<AcceptOrganizationInvitationCommand>
{
    private readonly IServiceManager _services;

    public AcceptOrganizationInvitationCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(AcceptOrganizationInvitationCommand request, CancellationToken _)
    {
        var employee = await _services.Employees.GetAsync(request.Jwt);
        var invitation = await _services.Organizations.Invitations.GetAsync(i => i.Id == request.InvitationId);
        var organization = await _services.Organizations.TryGetAsync(o => o.Id == invitation.OrganizationId) ?? throw new NotFound404Exception("The organization assigned to the invitation was not found.");

        if (invitation.Employee!.Id != employee.Id) throw new Unauthorized401Exception("The invitation is not assigned to you.");

        organization.AddEmployee(employee);

        await _services.Organizations.Invitations.DeleteAsync(invitation);
        await _services.Employees.UpdateAsync(employee);
        await _services.Organizations.UpdateAsync(organization);
    }
}