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
        var organization = await _services.Organizations.GetAsync(o => o.Id == request.OrganizationId);

        // Ensure the invitation belongs to the specified organization.
        if (invitation.OrganizationId != request.OrganizationId) throw new BadRequest400Exception("The invitation does not belong to the specified organization.");

        // Ensure the invitation is for the authenticated employee.
        if (invitation.Employee!.Id != employee.Id) throw new Unauthorized401Exception();

        // Ensure the employee is not already part of an organization.
        if (employee.OrganizationId is not null) throw new BadRequest400Exception("You are already part of an organization.");

        // Associate the employee with the organization.
        organization.Employees.Add(employee);
        employee.AssignOrganization(organization);

        // Delete the invitation
        await _services.Organizations.Invitations.DeleteAsync(invitation);
        organization.Invitations.Remove(invitation);

        // Save the changes.
        await _services.Organizations.UpdateAsync(organization);
        await _services.Employees.UpdateAsync(employee);

    }
}
