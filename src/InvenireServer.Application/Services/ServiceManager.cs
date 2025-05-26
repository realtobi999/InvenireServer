using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Domain.Core.Interfaces.Services;

namespace InvenireServer.Application.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IEmployeeService> _employees;

    public ServiceManager(IRepositoryManager repositories, IFactoryManager factories)
    {
        _employees = new Lazy<IEmployeeService>(() => new EmployeeService(repositories, factories));
    }

    public IEmployeeService Employees => _employees.Value;
}