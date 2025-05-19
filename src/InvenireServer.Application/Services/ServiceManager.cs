using InvenireServer.Domain.Core.Interfaces.Factories;
using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Domain.Core.Interfaces.Services;

namespace InvenireServer.Application.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IEmployeeService> _employee;

    public ServiceManager(IRepositoryManager repositories, IValidatorFactory factory)
    {
        _employee = new Lazy<IEmployeeService>(() => new EmployeeService(repositories, factory));
    }

    public IEmployeeService Employee => _employee.Value;
}