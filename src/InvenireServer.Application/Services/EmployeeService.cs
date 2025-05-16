using InvenireServer.Domain.Core.Entities;
using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Domain.Core.Interfaces.Services;

namespace InvenireServer.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IRepositoryManager _repositories;

    public EmployeeService(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task CreateAsync(Employee employee)
    {
        _repositories.Employee.Create(employee);
        await _repositories.SaveOrThrowAsync();
    }
}
