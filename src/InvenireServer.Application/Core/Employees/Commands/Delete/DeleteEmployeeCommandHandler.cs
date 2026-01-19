using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Employees.Commands.Delete;

/// <summary>
/// Handler for the request to delete an employee.
/// </summary>
public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand>
{
    private readonly IRepositoryManager _repositories;

    public DeleteEmployeeCommandHandler(IRepositoryManager services)
    {
        _repositories = services;
    }

    /// <summary>
    /// Handles the request to delete an employee.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task Handle(DeleteEmployeeCommand request, CancellationToken ct)
    {
        var employee = await _repositories.Employees.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The employee was not found in the system.");

        await _repositories.Employees.ExecuteDeleteAsync(employee);
    }
}
