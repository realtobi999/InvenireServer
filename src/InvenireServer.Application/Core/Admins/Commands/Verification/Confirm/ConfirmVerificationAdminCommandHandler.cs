using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Admins.Commands.Verification.Confirm;

public class ConfirmVerificationAdminCommandHandler : IRequestHandler<ConfirmVerificationAdminCommand>
{
    private readonly IRepositoryManager _repositories;

    public ConfirmVerificationAdminCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task Handle(ConfirmVerificationAdminCommand request, CancellationToken ct)
    {
        var purpose = request.Jwt.GetPurpose() ?? throw new BadRequest400Exception("The token's purpose is missing.");
        if (purpose != "email_verification") throw new Unauthorized401Exception("The token's purpose is not for email verification.");

        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");

        admin.Verify();

        await _repositories.Admins.ExecuteUpdateAsync(admin);
    }
}