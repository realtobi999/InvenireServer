using InvenireServer.Domain.Interfaces.Services.Admins;
using InvenireServer.Domain.Interfaces.Services.Employees;
using InvenireServer.Domain.Interfaces.Services.Organizations;
using InvenireServer.Domain.Interfaces.Services.Properties;

namespace InvenireServer.Application.Interfaces.Managers;

public interface IServiceManager
{
    IAdminService Admins { get; }

    IEmployeeService Employees { get; }

    IPropertyService Properties { get; }

    IOrganizationService Organizations { get; }
}