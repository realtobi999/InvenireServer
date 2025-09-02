using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;

public class CreateOrganizationInvitationCommandHandler : IRequestHandler<CreateOrganizationInvitationCommand, CreateOrganizationInvitationCommandResult>
{
    private readonly IRepositoryManager _repositories;

    public CreateOrganizationInvitationCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<CreateOrganizationInvitationCommandResult> Handle(CreateOrganizationInvitationCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");

        var employee = null as Employee;
        if (request.EmployeeId is not null)
            employee = await _repositories.Employees.GetAsync(e => e.Id == request.EmployeeId) ?? throw new NotFound404Exception("The employee was not found in the system.");
        else if (request.EmployeeEmailAddress is not null)
            employee = await _repositories.Employees.GetAsync(e => e.EmailAddress == request.EmployeeEmailAddress) ?? throw new NotFound404Exception("The employee was not found in the system.");

        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");

        if (await _repositories.Organizations.Invitations.GetAsync(i => i.Employee!.Id == employee!.Id && i.OrganizationId == organization.Id) is not null)
            throw new Conflict409Exception("The organization already has a invitation for the employee.");

        var invitation = new OrganizationInvitation
        {
            Id = request.Id ?? Guid.NewGuid(),
            Description = request.Description,
            CreatedAt = DateTimeOffset.UtcNow,
            LastUpdatedAt = null,
            Employee = employee,
        };

        if (await _repositories.Organizations.Invitations.CountAsync(i => i.OrganizationId == organization.Id) > Organization.MAX_INVITATIONS)
            throw new Conflict409Exception($"The organization's number of invitations exceeded the limit (max {Organization.MAX_INVITATIONS}).");
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