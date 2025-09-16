using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Employees.Commands.Delete;

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand>
{
    private readonly IRepositoryManager _repositories;

    public DeleteEmployeeCommandHandler(IRepositoryManager services)
    {
        _repositories = services;
    }

    public async Task Handle(DeleteEmployeeCommand request, CancellationToken ct)
    {
        var employee = await _repositories.Employees.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The employee was not found in the system.");

        await _repositories.Employees.ExecuteDeleteAsync(employee);
    }
}
