using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Interfaces.Services.Employees;

public interface IEmployeeService
{
    public IEmployeeDtoService Dto { get; }
    Task<IEnumerable<Employee>> IndexInactiveAsync();
    Task<Employee> GetAsync(Jwt jwt);
    Task<Employee> GetAsync(Expression<Func<Employee, bool>> predicate);
    Task CreateAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task UpdateAsync(IEnumerable<Employee> employees);
    Task DeleteAsync(Employee employee);
    Task DeleteAsync(IEnumerable<Employee> employees);
}

public interface IEmployeeDtoService
{
    Task<EmployeeDto> GetAsync(Jwt jwt);
    Task<EmployeeDto> GetAsync(Expression<Func<Employee, bool>> predicate);
}