using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Employees.Commands.Update;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand>
{
    private readonly IServiceManager _services;

    public UpdateEmployeeCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(UpdateEmployeeCommand request, CancellationToken _)
    {
        var employee = await _services.Employees.GetAsync(request.Jwt!);

        employee.Name = request.Name;

        await _services.Employees.UpdateAsync(employee);
    }
}
