using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Employees.Commands.Update;

/// <summary>
/// Handler for the request to update an employee.
/// </summary>
public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand>
{
    private readonly IRepositoryManager _repositories;

    public UpdateEmployeeCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the request to update an employee.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task Handle(UpdateEmployeeCommand request, CancellationToken ct)
    {
        var employee = await _repositories.Employees.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The employee was not found in the system.");

        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;

        await _repositories.Employees.ExecuteUpdateAsync(employee);
    }
}
