using InvenireServer.Application.Interfaces.Factories;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Services.Admins;
using InvenireServer.Application.Services.Employees;
using InvenireServer.Application.Services.Organizations;
using InvenireServer.Application.Services.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Interfaces.Services.Admins;
using InvenireServer.Domain.Interfaces.Services.Employees;
using InvenireServer.Domain.Interfaces.Services.Organizations;
using InvenireServer.Domain.Interfaces.Services.Properties;

namespace InvenireServer.Application.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IAdminService> _admins;
    private readonly Lazy<IEmployeeService> _employees;
    private readonly Lazy<IPropertyService> _properties;
    private readonly Lazy<IOrganizationService> _organizations;

    public ServiceManager(IRepositoryManager repositories, IValidatorFactory validators)
    {
        _admins = new Lazy<IAdminService>(() => new AdminService(repositories, validators.Initiate<Admin>()));
        _employees = new Lazy<IEmployeeService>(() => new EmployeeService(repositories, validators.Initiate<Employee>()));
        _properties = new Lazy<IPropertyService>(() => new PropertyService());
        _organizations = new Lazy<IOrganizationService>(() => new OrganizationService(repositories, validators));
    }

    public IAdminService Admins => _admins.Value;
    public IEmployeeService Employees => _employees.Value;
    public IPropertyService Properties => _properties.Value;
    public IOrganizationService Organizations => _organizations.Value;
}