using System.Security.Claims;
using InvenireServer.Application.Dtos.Employees.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Employees.Commands.Verification.Send;

/// <summary>
/// Handler for the request to send a verification for an employee.
/// </summary>
public class SendVerificationEmployeeCommandHandler : IRequestHandler<SendVerificationEmployeeCommand>
{
    private readonly IJwtManager _jwt;
    private readonly IEmailManager _email;
    private readonly IRepositoryManager _repositories;

    public SendVerificationEmployeeCommandHandler(IJwtManager jwt, IEmailManager email, IRepositoryManager repositories)
    {
        _jwt = jwt;
        _email = email;
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the request to send a verification for an employee.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task Handle(SendVerificationEmployeeCommand request, CancellationToken ct)
    {
        var employee = await _repositories.Employees.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The employee was not found in the system.");

        if (employee.IsVerified) throw new BadRequest400Exception("The employee's verification status is already confirmed.");

        var jwt = request.Jwt;
        jwt.Payload.Add(new Claim("purpose", "email_verification"));

        var dto = new EmployeeVerificationEmailDto
        {
            EmployeeAddress = employee.EmailAddress,
            EmployeeFirstName = employee.FirstName,
            VerificationLink = $"{request.FrontendBaseAddress}/verify-email?token={_jwt.Writer.Write(jwt)}"
        };
        var email = _email.Builders.Employee.BuildVerificationEmail(dto);
        await _email.Sender.SendEmailAsync(email);
    }
}