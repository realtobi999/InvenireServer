using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;

public class CreateOrganizationInvitationCommandHandler : IRequestHandler<CreateOrganizationInvitationCommand, CreateOrganizationInvitationCommandResult>
{
    private readonly IServiceManager _services;

    public CreateOrganizationInvitationCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task<CreateOrganizationInvitationCommandResult> Handle(CreateOrganizationInvitationCommand request, CancellationToken _)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt!);
        var organization = await _services.Organizations.TryGetAsync(o => o.Id == admin.OrganizationId) ?? throw new BadRequest400Exception("You have not created an organization. You must first create an organization before creating invitations.");
        var employee = await _services.Employees.GetAsync(e => e.Id == request.EmployeeId);

        // Create the invitation and assign the employee to it.
        var invitation = new OrganizationInvitation
        {
            Id = request.Id ?? Guid.NewGuid(),
            Description = request.Description,
            CreatedAt = DateTimeOffset.UtcNow,
            LastUpdatedAt = null
        };
        invitation.AssignEmployee(employee);
        organization.AddInvitation(invitation);

        // Save the changes.
        await _services.Organizations.Invitations.CreateAsync(invitation);
        await _services.Organizations.UpdateAsync(organization);

        return new CreateOrganizationInvitationCommandResult
        {
            Invitation = invitation,
            Organization = organization
        };
    }
}