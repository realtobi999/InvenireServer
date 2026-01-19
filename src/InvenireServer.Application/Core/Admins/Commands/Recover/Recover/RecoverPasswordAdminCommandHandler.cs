using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Application.Core.Admins.Commands.Recover.Recover;

/// <summary>
/// Handler for the request to recover a password for an admin.
/// </summary>
public class RecoverPasswordAdminCommandHandler : IRequestHandler<RecoverPasswordAdminCommand>
{
    private readonly IRepositoryManager _repositories;
    private readonly IPasswordHasher<Admin> _hasher;

    public RecoverPasswordAdminCommandHandler(IPasswordHasher<Admin> hasher, IRepositoryManager repositories)
    {
        _hasher = hasher;
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the request to recover a password for an admin.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task Handle(RecoverPasswordAdminCommand request, CancellationToken ct)
    {
        // Make sure that the token is intended for password recovery.
        var purpose = request.Jwt!.GetPurpose() ?? throw new BadRequest400Exception("The token's purpose is missing.");
        if (purpose != "password_recovery") throw new Unauthorized401Exception("The token's purpose is not for password recovery.");

        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");
        admin.Password = _hasher.HashPassword(admin, request.NewPassword);
        await _repositories.Admins.ExecuteUpdateAsync(admin);
    }
}
