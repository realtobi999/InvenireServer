using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Services.Admins;
using InvenireServer.Application.Services.Employees;
using InvenireServer.Domain.Interfaces.Services.Admins;
using InvenireServer.Domain.Interfaces.Services.Employees;
using Microsoft.Extensions.Configuration;

namespace InvenireServer.Application.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IAdminService> _admins;
    private readonly Lazy<IEmployeeService> _employees;

    public ServiceManager(IRepositoryManager repositories, IFactoryManager factories, IEmailManager email, IConfiguration configuration, IJwtManager jwt)
    {
        _admins = new Lazy<IAdminService>(() => new AdminService(repositories, email, jwt, configuration));
        _employees = new Lazy<IEmployeeService>(() => new EmployeeService(repositories, factories, email, jwt, configuration));
    }

    public IAdminService Admins => _admins.Value;

    public IEmployeeService Employees => _employees.Value;
}