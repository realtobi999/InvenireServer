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

    public async Task Handle(SendVerificationAdminCommand request, CancellationToken _)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt);

        // Make the purpose of the token to be of verification.
        var jwt = request.Jwt;
        jwt.Payload.Add(new Claim("purpose", "email_verification"));

        // Build and send the email.
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