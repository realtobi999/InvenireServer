using InvenireServer.Domain.Interfaces.Services.Admins;
using InvenireServer.Domain.Interfaces.Services.Employees;

namespace InvenireServer.Application.Interfaces.Managers;

public interface IServiceManager
{
    IAdminService Admins { get; }

    IEmployeeService Employees { get; }
}