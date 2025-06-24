using InvenireServer.Application.Dtos.Admins.Email;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Cqrs.Admins.Commands.Verification.Send;

public class SendVerificationAdminCommandHandler : IRequestHandler<SendVerificationAdminCommand, Unit>
{
    private readonly IJwtManager _jwt;
    private readonly IEmailManager _email;
    private readonly IServiceManager _services;

    public SendVerificationAdminCommandHandler(IServiceManager services, IEmailManager email, IJwtManager jwt)
    {
        _jwt = jwt;
        _email = email;
        _services = services;
    }

    public async Task<Unit> Handle(SendVerificationAdminCommand request, CancellationToken _)
    {
        var jwt = request.Jwt;
        var admin = await _services.Admins.GetAsync(jwt);

        // Make the purpose of the token to be of verification.
        jwt.Payload.Add(new("purpose", "email_verification"));

        // Build and send the email.
        var dto = new AdminVerificationEmailDto
        {
            AdminAddress = admin.EmailAddress,
            AdminName = admin.Name,
            VerificationLink = $"{request.FrontendBaseUrl}/verify-email?token={_jwt.Writer.Write(jwt)}"
        };
        var email = _email.Builders.Admin.BuildVerificationEmail(dto);
        await _email.Sender.SendEmailAsync(email);

        return Unit.Value;
    }
}
