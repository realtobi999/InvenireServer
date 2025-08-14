using System.Security.Claims;
using InvenireServer.Application.Dtos.Admins.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Admins.Commands.Verification.Send;

public class SendVerificationAdminCommandHandler : IRequestHandler<SendVerificationAdminCommand>
{
    private readonly IJwtManager _jwt;
    private readonly IEmailManager _email;
    private readonly IRepositoryManager _repositories;

    public SendVerificationAdminCommandHandler(IJwtManager jwt, IEmailManager email, IRepositoryManager repositories)
    {
        _jwt = jwt;
        _email = email;
        _repositories = repositories;
    }

    public async Task Handle(SendVerificationAdminCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");

        if (admin.IsVerified) throw new BadRequest400Exception("The admin's verification status is already confirmed.");

        var jwt = request.Jwt;
        jwt.Payload.Add(new Claim("purpose", "email_verification"));

        var dto = new AdminVerificationEmailDto
        {
            AdminAddress = admin.EmailAddress,
            AdminName = admin.FirstName,
            VerificationLink = $"{request.FrontendBaseUrl}/verify-email?token={_jwt.Writer.Write(jwt)}"
        };
        var email = _email.Builders.Admin.BuildVerificationEmail(dto);
        await _email.Sender.SendEmailAsync(email);
    }
}