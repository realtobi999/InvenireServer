using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Domain.Core.Interfaces.Services;

namespace InvenireServer.Application.Services;

/// <summary>
/// Central access point for application services, providing lazy initialization of services.
/// </summary>
public class ServiceManager : IServiceManager
{
    private readonly Lazy<IEmployeeService> _employees;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceManager"/> class.
    /// </summary>
    /// <param name="repositories">Provides access to repository operations.</param>
    /// <param name="factories">Provides access to factory-created services like validators and JWT tools.</param>
    /// <param name="email">Handles email-related functionality for services.</param>
    public ServiceManager(IRepositoryManager repositories, IFactoryManager factories, IEmailManager email)
    {
        _employees = new Lazy<IEmployeeService>(() => new EmployeeService(repositories, factories, email));
    }

    /// <inheritdoc/>
    public IEmployeeService Employees => _employees.Value;
}
