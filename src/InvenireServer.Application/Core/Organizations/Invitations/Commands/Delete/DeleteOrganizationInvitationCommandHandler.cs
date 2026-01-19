
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Delete;

/// <summary>
/// Handler for the request to delete an organization invitation.
/// </summary>
public class DeleteOrganizationInvitationCommandHandler : IRequestHandler<DeleteOrganizationInvitationCommand>
{
    private readonly IRepositoryManager _repositories;

    public DeleteOrganizationInvitationCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the request to delete an organization invitation.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task Handle(DeleteOrganizationInvitationCommand request, CancellationToken ct)
    {
        switch (request.Jwt.GetRole())
        {
            case Jwt.Roles.ADMIN:
                await DeleteInvitationAsAdminAsync(request);
                break;
            case Jwt.Roles.EMPLOYEE:
                await DeleteInvitationAsEmployeeAsync(request);
                break;
            default:
                throw new Unauthorized401Exception();
        }

        await _repositories.SaveOrThrowAsync();
    }

    private async Task DeleteInvitationAsAdminAsync(DeleteOrganizationInvitationCommand request)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var invitation = await _repositories.Organizations.Invitations.GetAsync(i => i.Id == request.Id) ?? throw new NotFound404Exception("The invitation was not found in the system.");

        if (invitation.OrganizationId != organization.Id) throw new Unauthorized401Exception("The invitation isn't part of the organization.");
        organization.RemoveInvitation(invitation);

        _repositories.Organizations.Update(organization);
        _repositories.Organizations.Invitations.Delete(invitation);
    }

    private async Task DeleteInvitationAsEmployeeAsync(DeleteOrganizationInvitationCommand request)
    {
        var employee = await _repositories.Employees.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The employee was not found in the system.");
        var invitation = await _repositories.Organizations.Invitations.GetAsync(i => i.Id == request.Id) ?? throw new NotFound404Exception("The invitation was not found in the system.");
        var organization = await _repositories.Organizations.GetAsync(o => o.Id == invitation.OrganizationId) ?? throw new NotFound404Exception("The organization assigned to the invitation was not found in the system.");

        if (invitation.Employee!.Id != employee.Id) throw new Unauthorized401Exception("The invitation doesn't belong to the employee.");

        if (invitation.OrganizationId != organization.Id) throw new Unauthorized401Exception("The invitation isn't part of the organization.");
        organization.RemoveInvitation(invitation);

        _repositories.Organizations.Update(organization);
        _repositories.Organizations.Invitations.Delete(invitation);
    }
}
