using InvenireServer.Application.Dtos.Employees.Email;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Employees.Commands.Verification.Send;

public class SendVerificationEmployeeCommandHandler : IRequestHandler<SendVerificationEmployeeCommand>
{
    private readonly IJwtManager _jwt;
    private readonly IEmailManager _email;
    private readonly IServiceManager _services;

    public SendVerificationEmployeeCommandHandler(IJwtManager jwt, IEmailManager email, IServiceManager services)
    {
        _jwt = jwt;
        _email = email;
        _services = services;
    }

    public async Task Handle(SendVerificationEmployeeCommand request, CancellationToken _)
    {
        var jwt = request.Jwt;
        var employee = await _services.Employees.GetAsync(jwt);

        // Make the purpose of the token to be of verification.
        jwt.Payload.Add(new("purpose", "email_verification"));

        // Build and send the email.
        var dto = new EmployeeVerificationEmailDto
        {
            EmployeeAddress = employee.EmailAddress,
            EmployeeName = employee.Name,
            VerificationLink = $"{request.FrontendBaseUrl}/verify-email?token={_jwt.Writer.Write(jwt)}"
        };
        var email = _email.Builders.Employee.BuildVerificationEmail(dto);
        await _email.Sender.SendEmailAsync(email);
    }
}
