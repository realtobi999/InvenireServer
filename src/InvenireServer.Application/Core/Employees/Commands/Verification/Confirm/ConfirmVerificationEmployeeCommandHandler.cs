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

    public async Task Handle(ConfirmVerificationEmployeeCommand request, CancellationToken ct)
    {
        var purpose = request.Jwt.GetPurpose() ?? throw new BadRequest400Exception("The token's purpose is missing.");
        if (purpose != "email_verification") throw new Unauthorized401Exception("The token's purpose is not for email verification.");

        var employee = await _services.Employees.GetAsync(request.Jwt);

        employee.Verify();

        await _services.Employees.UpdateAsync(employee);
    }
}