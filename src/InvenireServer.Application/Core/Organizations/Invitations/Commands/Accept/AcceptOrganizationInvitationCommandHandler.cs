using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Accept;

/// <summary>
/// Handler for the request to accept an organization invitation.
/// </summary>
public class AcceptOrganizationInvitationCommandHandler : IRequestHandler<AcceptOrganizationInvitationCommand>
{
    private readonly IRepositoryManager _repositories;

    public AcceptOrganizationInvitationCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the request to accept an organization invitation.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task Handle(AcceptOrganizationInvitationCommand request, CancellationToken ct)
    {
        var employee = await _repositories.Employees.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The employee was not found in the system.");
        var invitation = await _repositories.Organizations.Invitations.GetAsync(i => i.Id == request.InvitationId) ?? throw new NotFound404Exception("The invitation was not found in the system.");
        var organization = await _repositories.Organizations.GetAsync(o => o.Id == invitation.OrganizationId) ?? throw new NotFound404Exception("The organization assigned to the invitation was not found in the system.");

        if (invitation.Employee!.Id != employee.Id) throw new Unauthorized401Exception("The invitation is not assigned to the employee.");
        organization.AddEmployee(employee);

        _repositories.Employees.Update(employee);
        _repositories.Organizations.Update(organization);
        _repositories.Organizations.Invitations.Delete(invitation);

        await _repositories.SaveOrThrowAsync();
    }
}