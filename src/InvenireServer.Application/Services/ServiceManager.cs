using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Services.Admins;
using InvenireServer.Application.Services.Employees;
using InvenireServer.Domain.Interfaces.Services.Admins;
using InvenireServer.Domain.Interfaces.Services.Employees;
using Microsoft.Extensions.Configuration;

namespace InvenireServer.Application.Services;

/// <summary>
/// Central access point for application services, providing lazy initialization of services.
/// </summary>
public class ServiceManager : IServiceManager
{
    private readonly Lazy<IAdminService> _admins;
    private readonly Lazy<IEmployeeService> _employees;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceManager"/> class with the required dependencies.
    /// </summary>
    /// <param name="repositories">Repository manager for data access operations.</param>
    /// <param name="factories">Factory manager used to initiate validators and other factory-bound logic.</param>
    /// <param name="email">Email manager for sending emails.</param>
    /// <param name="configuration">Application configuration provider.</param>
    /// <param name="jwt">JWT manager for handling token creation and parsing.</param>
    public ServiceManager(IRepositoryManager repositories, IFactoryManager factories, IEmailManager email, IConfiguration configuration, IJwtManager jwt)
    {
        _admins = new Lazy<IAdminService>(() => new AdminService(repositories, email, jwt, configuration));
        _employees = new Lazy<IEmployeeService>(() => new EmployeeService(repositories, factories, email, jwt, configuration));
    }

    /// <inheritdoc/>
    public IAdminService Admins => _admins.Value;

    /// <inheritdoc/>
    public IEmployeeService Employees => _employees.Value;
}