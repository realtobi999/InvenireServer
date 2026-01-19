using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Employees.Commands.Verification.Confirm;

/// <summary>
/// Handler for the request to confirm a verification for an employee.
/// </summary>
public class ConfirmVerificationEmployeeCommandHandler : IRequestHandler<ConfirmVerificationEmployeeCommand>
{
    private readonly IRepositoryManager _repositories;

    public ConfirmVerificationEmployeeCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the request to confirm a verification for an employee.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task Handle(ConfirmVerificationEmployeeCommand request, CancellationToken ct)
    {
        var purpose = request.Jwt.GetPurpose() ?? throw new BadRequest400Exception("The token's purpose is missing.");
        if (purpose != "email_verification") throw new Unauthorized401Exception("The token's purpose is not for email verification.");

        var employee = await _repositories.Employees.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The employee was not found in the system.");

        employee.Verify();

        await _repositories.Employees.ExecuteUpdateAsync(employee);
    }
}