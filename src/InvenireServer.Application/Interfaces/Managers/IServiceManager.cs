using InvenireServer.Application.Interfaces.Services.Admins;
using InvenireServer.Application.Interfaces.Services.Employees;
using InvenireServer.Application.Interfaces.Services.Organizations;
using InvenireServer.Application.Interfaces.Services.Properties;

namespace InvenireServer.Application.Interfaces.Managers;

public interface IServiceManager
{
    IAdminService Admins { get; }

    IEmployeeService Employees { get; }

    IPropertyService Properties { get; }

    IOrganizationService Organizations { get; }
}