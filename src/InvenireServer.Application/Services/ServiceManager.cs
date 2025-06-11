using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Services.Admins;
using InvenireServer.Application.Services.Employees;
using InvenireServer.Application.Services.Organizations;
using InvenireServer.Domain.Interfaces.Services.Admins;
using InvenireServer.Domain.Interfaces.Services.Employees;
using InvenireServer.Domain.Interfaces.Services.Organizations;
using Microsoft.Extensions.Configuration;

namespace InvenireServer.Application.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IAdminService> _admins;
    private readonly Lazy<IEmployeeService> _employees;
    private readonly Lazy<IOrganizationService> _organizations;

    public ServiceManager(IRepositoryManager repositories, IFactoryManager factories, IEmailManager email, IConfiguration configuration, IJwtManager jwt)
    {
        _admins = new Lazy<IAdminService>(() => new AdminService(repositories, email, jwt, configuration));
        _employees = new Lazy<IEmployeeService>(() => new EmployeeService(repositories, factories, email, jwt, configuration));
        _organizations = new Lazy<IOrganizationService>(() => new OrganizationService(repositories));
    }

    public IAdminService Admins => _admins.Value;

    public IEmployeeService Employees => _employees.Value;

    public IOrganizationService Organizations => _organizations.Value;
}