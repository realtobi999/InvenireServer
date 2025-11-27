using System.Security.Claims;
using InvenireServer.Application.Dtos.Admins.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Admins.Commands.Recover.Send;

public class SendPasswordRecoveryAdminCommandHandler : IRequestHandler<SendPasswordRecoveryAdminCommand>
{
    private readonly IJwtManager _jwt;
    private readonly IEmailManager _email;
    private readonly IRepositoryManager _repositories;

    public SendPasswordRecoveryAdminCommandHandler(IJwtManager jwt, IEmailManager email, IRepositoryManager repositories)
    {
        _jwt = jwt;
        _email = email;
        _repositories = repositories;
    }

    public async Task Handle(SendPasswordRecoveryAdminCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(a => a.EmailAddress == request.EmailAddress) ?? throw new NotFound404Exception("Admin was not found in the system.");

        var jwt = _jwt.Builder.Build(
        [
            new Claim("role", Jwt.Roles.ADMIN),
            new Claim("admin_id", admin.Id.ToString()),
            new Claim("purpose", "password_recovery")
        ]);

        var email = _email.Builders.Admin.BuildRecoveryEmail(new AdminRecoveryEmailDto
        {
            AdminAddress = admin.EmailAddress,
            AdminFirstName = admin.FirstName,
            RecoveryLink = $"{request.FrontendBaseAddress}/password-recovery/recover?token={_jwt.Writer.Write(jwt)}"
        });
        await _email.Sender.SendEmailAsync(email);
    }
}
