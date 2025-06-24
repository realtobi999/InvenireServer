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
        var employee = await _services.Employees.GetAsync(e => e.Id == request.EmployeeId);
        var organization = await _services.Organizations.GetAsync(o => o.Id == request.OrganizationId);

        // Ensure the admin is the owner of the organization.
        if (admin.Id != organization.Admin!.Id) throw new Unauthorized401Exception();

        // Create and associate the invitation.
        var invitation = new OrganizationInvitation
        {
            Id = request.Id ?? Guid.NewGuid(),
            Description = request.Description,
            CreatedAt = DateTimeOffset.UtcNow,
            LastUpdatedAt = null,
            Employee = employee,
            OrganizationId = organization.Id
        };

        // Save the invitation.
        await _services.Organizations.Invitations.CreateAsync(invitation);

        // Add invitation to the organization.
        organization.Invitations?.Add(invitation);

        return new CreateOrganizationInvitationCommandResult
        {
            Invitation = invitation,
            Organization = organization
        };
    }
}