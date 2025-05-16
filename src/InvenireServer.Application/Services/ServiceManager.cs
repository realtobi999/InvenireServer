using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Domain.Core.Interfaces.Services;

namespace InvenireServer.Application.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IEmployeeService> _employee;

    public ServiceManager(IRepositoryManager repositories)
    {
        _employee = new Lazy<IEmployeeService>(() => new EmployeeService(repositories));
    }

    public IEmployeeService Employee => _employee.Value;
}