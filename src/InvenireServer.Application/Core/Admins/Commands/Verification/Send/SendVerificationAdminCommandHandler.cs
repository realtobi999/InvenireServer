using System.Security.Claims;
using InvenireServer.Application.Dtos.Admins.Email;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Admins.Commands.Verification.Send;

public class SendVerificationAdminCommandHandler : IRequestHandler<SendVerificationAdminCommand>
{
    private readonly IEmailManager _email;
    private readonly IJwtManager _jwt;
    private readonly IServiceManager _services;

    public SendVerificationAdminCommandHandler(IServiceManager services, IEmailManager email, IJwtManager jwt)
    {
        _jwt = jwt;
        _email = email;
        _services = services;
    }

    public async Task Handle(SendVerificationAdminCommand request, CancellationToken ct)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt);

        var jwt = request.Jwt;
        jwt.Payload.Add(new Claim("purpose", "email_verification"));

        var dto = new AdminVerificationEmailDto
        {
            AdminAddress = admin.EmailAddress,
            AdminName = admin.Name,
            VerificationLink = $"{request.FrontendBaseUrl}/verify-email?token={_jwt.Writer.Write(jwt)}"
        };
        var email = _email.Builders.Admin.BuildVerificationEmail(dto);
        await _email.Sender.SendEmailAsync(email);
    }
}