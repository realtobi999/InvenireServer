using InvenireServer.Application.Dtos.Admins.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Commands.Create;

public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, CreateOrganizationCommandResult>
{
    private readonly IEmailManager _email;
    private readonly IRepositoryManager _repositories;

    public CreateOrganizationCommandHandler(IEmailManager email, IRepositoryManager repositories)
    {
        _email = email;
        _repositories = repositories;
    }

    public async Task<CreateOrganizationCommandResult> Handle(CreateOrganizationCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");

        var organization = new Organization
        {
            Id = request.Id ?? Guid.NewGuid(),
            Name = request.Name,
            CreatedAt = DateTimeOffset.UtcNow,
            LastUpdatedAt = null,
        };
        organization.AssignAdmin(admin);

        _repositories.Admins.Update(admin);
        _repositories.Organizations.Create(organization);

        await _repositories.SaveOrThrowAsync();

        var dto = new AdminOrganizationCreationEmailDto
        {
            AdminAddress = admin.EmailAddress,
            AdminFirstName = admin.FirstName,
            OrganizationName = organization.Name,
            DashboardLink = $"{request.FrontendBaseAddress}/dashboard"
        };
        var email = _email.Builders.Admin.BuildOrganizationCreationEmail(dto);
        await _email.Sender.SendEmailAsync(email);

        return new CreateOrganizationCommandResult
        {
            Organization = organization
        };
    }
}