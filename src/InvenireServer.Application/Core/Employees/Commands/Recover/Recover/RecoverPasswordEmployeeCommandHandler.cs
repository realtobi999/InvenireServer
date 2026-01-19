using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Application.Core.Employees.Commands.Recover.Recover;

/// <summary>
/// Handler for the request to recover a password for an employee.
/// </summary>
public class RecoverPasswordEmployeeCommandHandler : IRequestHandler<RecoverPasswordEmployeeCommand>
{
    private readonly IRepositoryManager _repositories;
    private readonly IPasswordHasher<Employee> _hasher;

    public RecoverPasswordEmployeeCommandHandler(IPasswordHasher<Employee> hasher, IRepositoryManager repositories)
    {
        _hasher = hasher;
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the request to recover a password for an employee.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task Handle(RecoverPasswordEmployeeCommand request, CancellationToken ct)
    {
        // Make sure that the token is intended for password recovery.
        var purpose = request.Jwt!.GetPurpose() ?? throw new BadRequest400Exception("The token's purpose is missing.");
        if (purpose != "password_recovery") throw new Unauthorized401Exception("The token's purpose is not for password recovery.");

        var employee = await _repositories.Employees.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The employee was not found in the system.");
        employee.Password = _hasher.HashPassword(employee, request.NewPassword);
        await _repositories.Employees.ExecuteUpdateAsync(employee);
    }
}
