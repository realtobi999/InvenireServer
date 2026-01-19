using System.Security.Claims;
using InvenireServer.Application.Dtos.Employees.Email;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Employees.Commands.Recover.Send;

/// <summary>
/// Handler for the request to send a password recovery for an employee.
/// </summary>
public class SendPasswordRecoveryEmployeeCommandHandler : IRequestHandler<SendPasswordRecoveryEmployeeCommand>
{
    private readonly IJwtManager _jwt;
    private readonly IEmailManager _email;
    private readonly IRepositoryManager _repositories;

    public SendPasswordRecoveryEmployeeCommandHandler(IJwtManager jwt, IEmailManager email, IRepositoryManager repositories)
    {
        _jwt = jwt;
        _email = email;
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the request to send a password recovery for an employee.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task Handle(SendPasswordRecoveryEmployeeCommand request, CancellationToken ct)
    {
        var employee = await _repositories.Employees.GetAsync(a => a.EmailAddress == request.EmailAddress) ?? throw new NotFound404Exception("The employee was not found in the system.");

        var jwt = _jwt.Builder.Build(
        [
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("purpose", "password_recovery")
        ]);

        var email = _email.Builders.Employee.BuildRecoveryEmail(new EmployeeRecoveryEmailDto
        {
            EmployeeAddress = employee.EmailAddress,
            RecoveryLink = $"{request.FrontendBaseAddress}/password-recovery/recover?token={_jwt.Writer.Write(jwt)}"
        });
        await _email.Sender.SendEmailAsync(email);
    }
}
