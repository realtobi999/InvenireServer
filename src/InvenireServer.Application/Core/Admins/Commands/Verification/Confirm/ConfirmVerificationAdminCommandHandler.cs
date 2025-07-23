using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Admins.Commands.Verification.Confirm;

public class ConfirmVerificationAdminCommandHandler : IRequestHandler<ConfirmVerificationAdminCommand>
{
    private readonly IServiceManager _services;

    public ConfirmVerificationAdminCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(ConfirmVerificationAdminCommand request, CancellationToken ct)
    {
        var purpose = request.Jwt.GetPurpose() ?? throw new BadRequest400Exception("The token's purpose is missing.");
        if (purpose != "email_verification") throw new Unauthorized401Exception("The token's purpose is not for email verification.");

        var admin = await _services.Admins.GetAsync(request.Jwt);

        admin.Verify();

        await _services.Admins.UpdateAsync(admin);
    }
}