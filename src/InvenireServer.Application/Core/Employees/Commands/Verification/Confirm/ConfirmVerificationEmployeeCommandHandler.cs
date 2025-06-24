using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Employees.Commands.Verification.Confirm;

public class ConfirmVerificationEmployeeCommandHandler : IRequestHandler<ConfirmVerificationEmployeeCommand>
{
    private readonly IServiceManager _services;

    public ConfirmVerificationEmployeeCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(ConfirmVerificationEmployeeCommand request, CancellationToken _)
    {
        // Make sure that the token purpose is for verification.
        var jwt = request.Jwt;
        if (!jwt.Payload.Any(c => c.Type == "purpose" && c.Value == "email_verification")) throw new Unauthorized401Exception("Indication that the token is used for email verification is missing.");

        // Verify the employee and save changes to the databases.
        var employee = await _services.Employees.GetAsync(jwt);
        employee.Verify();
        await _services.Employees.UpdateAsync(employee);
    }
}