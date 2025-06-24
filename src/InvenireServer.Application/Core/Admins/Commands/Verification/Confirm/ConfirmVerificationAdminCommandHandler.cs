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

    public async Task Handle(ConfirmVerificationAdminCommand request, CancellationToken _)
    {
        // Make sure that the token purpose is for verification.
        var jwt = request.Jwt;
        if (!jwt.Payload.Any(c => c.Type == "purpose" && c.Value == "email_verification")) throw new Unauthorized401Exception("Indication that the token is used for email verification is missing.");

        // Verify the admin and save changes to the databases.
        var admin = await _services.Admins.GetAsync(jwt);
        admin.Verify();
        await _services.Admins.UpdateAsync(admin);
    }
}