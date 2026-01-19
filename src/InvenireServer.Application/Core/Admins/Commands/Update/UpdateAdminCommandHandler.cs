using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Admins.Commands.Update;

/// <summary>
/// Handler for the request to update an admin.
/// </summary>
public class UpdateAdminCommandHandler : IRequestHandler<UpdateAdminCommand>
{
    private readonly IRepositoryManager _repositories;

    public UpdateAdminCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the request to update an admin.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task Handle(UpdateAdminCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");

        admin.FirstName = request.FirstName;
        admin.LastName = request.LastName;

        await _repositories.Admins.ExecuteUpdateAsync(admin);
    }
}
