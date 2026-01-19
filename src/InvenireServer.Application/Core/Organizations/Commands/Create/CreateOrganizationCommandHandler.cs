using InvenireServer.Application.Dtos.Admins.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Commands.Create;

/// <summary>
/// Handler for the request to create an organization.
/// </summary>
public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, CreateOrganizationCommandResult>
{
    private readonly IRepositoryManager _repositories;

    public CreateOrganizationCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the request to create an organization.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task returning the response.</returns>
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
        admin.OrganizationId = organization.Id;

        _repositories.Admins.Update(admin);
        _repositories.Organizations.Create(organization);

        await _repositories.SaveOrThrowAsync();

        return new CreateOrganizationCommandResult
        {
            Organization = organization
        };
    }
}