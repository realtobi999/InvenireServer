using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Services.Admins;
using InvenireServer.Application.Services.Employees;
using InvenireServer.Application.Services.Organizations;
using InvenireServer.Application.Services.Properties;
using InvenireServer.Domain.Interfaces.Services.Admins;
using InvenireServer.Domain.Interfaces.Services.Employees;
using InvenireServer.Domain.Interfaces.Services.Organizations;
using InvenireServer.Domain.Interfaces.Services.Properties;

namespace InvenireServer.Application.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IAdminService> _admins;
    private readonly Lazy<IEmployeeService> _employees;
    private readonly Lazy<IOrganizationService> _organizations;
    private readonly Lazy<IPropertyService> _properties;

    public ServiceManager(IRepositoryManager repositories)
    {
        _admins = new Lazy<IAdminService>(() => new AdminService(repositories));
        _employees = new Lazy<IEmployeeService>(() => new EmployeeService(repositories));
        _properties = new Lazy<IPropertyService>(() => new PropertyService(repositories));
        _organizations = new Lazy<IOrganizationService>(() => new OrganizationService(repositories));
    }

    public IAdminService Admins => _admins.Value;
    public IEmployeeService Employees => _employees.Value;
    public IPropertyService Properties => _properties.Value;
    public IOrganizationService Organizations => _organizations.Value;
}