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
        var organization = await _services.Organizations.GetAsync(o => o.Id == request.OrganizationId);
        var invitation = await _services.Organizations.Invitations.TryGetAsync(i => i.OrganizationId == organization.Id && i.Employee!.Id == employee.Id) ?? throw new NotFound404Exception("There is no invitation for you to join this organization.");

        // Add the employee to the organization and delete the invitation.
        organization.AddEmployee(employee);
        await _services.Organizations.Invitations.DeleteAsync(invitation);

        // Save changes to the database.
        await _services.Employees.UpdateAsync(employee);
        await _services.Organizations.UpdateAsync(organization);
    }
}