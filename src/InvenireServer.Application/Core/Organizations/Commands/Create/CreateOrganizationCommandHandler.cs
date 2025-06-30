using InvenireServer.Application.Dtos.Admins.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Core.Organizations.Commands.Create;

public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, CreateOrganizationCommandResult>
{
    private readonly IEmailManager _email;
    private readonly IServiceManager _services;

    public CreateOrganizationCommandHandler(IServiceManager services, IEmailManager email)
    {
        _services = services;
        _email = email;
    }

    public async Task<CreateOrganizationCommandResult> Handle(CreateOrganizationCommand request, CancellationToken _)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt!);

        // Create the organization and assign the admin.
        var organization = new Organization
        {
            Id = request.Id ?? Guid.NewGuid(),
            Name = request.Name,
            CreatedAt = DateTimeOffset.UtcNow,
            LastUpdatedAt = null,
        };
        organization.AssignAdmin(admin);

        // Save the changes.
        await _services.Organizations.CreateAsync(organization);
        await _services.Admins.UpdateAsync(admin);

        // Send confirmation email to the admin.
        var dto = new AdminOrganizationCreationEmailDto
        {
            AdminAddress = admin.EmailAddress,
            AdminName = admin.Name,
            OrganizationName = organization.Name,
            DashboardLink = $"{request.FrontendBaseUrl}/dashboard"
        };
        var email = _email.Builders.Admin.BuildOrganizationCreationEmail(dto);
        await _email.Sender.SendEmailAsync(email);

        return new CreateOrganizationCommandResult
        {
            Organization = organization
        };
    }
}