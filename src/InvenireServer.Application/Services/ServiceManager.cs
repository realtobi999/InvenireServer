using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Domain.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;

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
    /// <param name="repositories">The repository manager for data access dependencies.</param>
    /// <param name="factories">The factory manager used to resolve validators and JWT utilities.</param>
    /// <param name="email">The email manager used for building and sending messages.</param>
    /// <param name="configuration">The application configuration provider.</param>
    public ServiceManager(IRepositoryManager repositories, IFactoryManager factories, IEmailManager email, IConfiguration configuration)
    {
        _employees = new Lazy<IEmployeeService>(() => new EmployeeService(repositories, factories, email, configuration));
    }

    /// <inheritdoc/>
    public IEmployeeService Employees => _employees.Value;
}
