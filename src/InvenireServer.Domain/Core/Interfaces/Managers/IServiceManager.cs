using InvenireServer.Domain.Core.Interfaces.Services;

namespace InvenireServer.Domain.Core.Interfaces.Managers;

public interface IServiceManager
{
    IEmployeeService Employees { get; }
}