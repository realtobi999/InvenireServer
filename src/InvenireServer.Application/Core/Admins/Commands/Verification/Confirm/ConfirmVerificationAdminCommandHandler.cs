using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Admins.Commands.Verification.Confirm;

/// <summary>
/// Handler for the request to confirm a verification for an admin.
/// </summary>
public class ConfirmVerificationAdminCommandHandler : IRequestHandler<ConfirmVerificationAdminCommand>
{
    private readonly IRepositoryManager _repositories;

    public ConfirmVerificationAdminCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the request to confirm a verification for an admin.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task Handle(ConfirmVerificationAdminCommand request, CancellationToken ct)
    {
        // Make sure that the token is intended for email verification.
        var purpose = request.Jwt.GetPurpose() ?? throw new BadRequest400Exception("The token's purpose is missing.");
        if (purpose != "email_verification") throw new Unauthorized401Exception("The token's purpose is not for email verification.");

        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");
        admin.Verify();
        await _repositories.Admins.ExecuteUpdateAsync(admin);
    }
}