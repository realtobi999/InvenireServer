using System.Security.Claims;
using InvenireServer.Application.Dtos.Admins.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Admins.Commands.Verification.Send;

/// <summary>
/// Handler for the request to send a verification for an admin.
/// </summary>
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

    /// <summary>
    /// Handles the request to send a verification for an admin.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task Handle(SendVerificationAdminCommand request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");
        if (admin.IsVerified) throw new BadRequest400Exception("The admin's verification status is already confirmed.");

        var jwt = request.Jwt;
        jwt.Payload.Add(new Claim("purpose", "email_verification"));

        var email = _email.Builders.Admin.BuildVerificationEmail(new AdminVerificationEmailDto
        {
            AdminAddress = admin.EmailAddress,
            AdminFirstName = admin.FirstName,
            VerificationLink = $"{request.FrontendBaseAddress}/verify-email?token={_jwt.Writer.Write(jwt)}"
        });
        await _email.Sender.SendEmailAsync(email);
    }
}