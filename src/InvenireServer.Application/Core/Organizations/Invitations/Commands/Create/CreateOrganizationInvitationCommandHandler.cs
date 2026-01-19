using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;

/// <summary>
/// Handler for the request to create an organization invitation.
/// </summary>
public class CreateOrganizationInvitationCommandHandler : IRequestHandler<CreateOrganizationInvitationCommand, CreateOrganizationInvitationCommandResult>
{
    private readonly IRepositoryManager _repositories;

    public CreateOrganizationInvitationCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the request to create an organization invitation.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task returning the response.</returns>
    public async Task<CreateOrganizationInvitationCommandResult> Handle(CreateOrganizationInvitationCommand request, CancellationToken ct)
    {
        var employee = request switch
        {
            { EmployeeId: not null } =>
                await _repositories.Employees.GetAsync(e => e.Id == request.EmployeeId)
                ?? throw new NotFound404Exception("The employee was not found in the system."),

            { EmployeeEmailAddress: not null } =>
                await _repositories.Employees.GetAsync(e => e.EmailAddress == request.EmployeeEmailAddress)
                ?? throw new NotFound404Exception("The employee was not found in the system."),

            _ => throw new BadRequest400Exception("Either 'employee_email_address' or 'employee_id' must be provided.") // This is already validated in the command validator.
        };

        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");

        if (await _repositories.Organizations.Invitations.CountAsync(i => i.Employee!.Id == employee.Id && i.OrganizationId == organization.Id) != 0)
            throw new Conflict409Exception("The organization already has a invitation for the employee.");
        if (await _repositories.Organizations.Invitations.CountAsync(i => i.OrganizationId == organization.Id) > Organization.MAX_INVITATIONS)
            throw new Conflict409Exception($"The organization's number of invitations exceeded the limit (max {Organization.MAX_INVITATIONS}).");

        var invitation = new OrganizationInvitation
        {
            Id = request.Id ?? Guid.NewGuid(),
            Description = request.Description,
            CreatedAt = DateTimeOffset.UtcNow,
            LastUpdatedAt = null,
            Employee = employee,
        };
        organization.AddInvitation(invitation);

        _repositories.Organizations.Update(organization);
        _repositories.Organizations.Invitations.Create(invitation);

        await _repositories.SaveOrThrowAsync();

        return new CreateOrganizationInvitationCommandResult
        {
            Invitation = invitation,
        };
    }
}