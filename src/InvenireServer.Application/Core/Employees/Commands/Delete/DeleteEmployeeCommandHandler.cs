using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Employees.Commands.Delete;

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand>
{
    private readonly IServiceManager _services;

    public DeleteEmployeeCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(DeleteEmployeeCommand request, CancellationToken _)
    {
        var employee = await _services.Employees.GetAsync(request.Jwt);

        await _services.Employees.DeleteAsync(employee);
    }
}
