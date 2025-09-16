using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Employees.Commands.Update;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand>
{
    private readonly IRepositoryManager _repositories;

    public UpdateEmployeeCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task Handle(UpdateEmployeeCommand request, CancellationToken ct)
    {
        var employee = await _repositories.Employees.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The employee was not found in the system.");

        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;

        await _repositories.Employees.ExecuteUpdateAsync(employee);
    }
}
