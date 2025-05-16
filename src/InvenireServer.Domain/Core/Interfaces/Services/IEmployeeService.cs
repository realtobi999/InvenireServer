using InvenireServer.Domain.Core.Entities;

namespace InvenireServer.Domain.Core.Interfaces.Services;

public interface IEmployeeService
{
    Task CreateAsync(Employee employee);
}
