using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Commands.Update;

/// <summary>
/// Handler for the request to update an organization.
/// </summary>
public class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand>
{
    private readonly IRepositoryManager _repositories;

    public UpdateOrganizationCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the request to update an organization.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task Handle(UpdateOrganizationCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");

        organization.Name = request.Name;

        await _repositories.Organizations.ExecuteUpdateAsync(organization);
    }
}
