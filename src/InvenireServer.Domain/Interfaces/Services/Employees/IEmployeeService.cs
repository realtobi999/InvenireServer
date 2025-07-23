using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Domain.Interfaces.Services.Employees;

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> IndexInactiveAsync();
    Task<Employee> GetAsync(Jwt jwt);
    Task<Employee> GetAsync(Expression<Func<Employee, bool>> predicate);
    Task CreateAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task UpdateAsync(IEnumerable<Employee> employees);
    Task DeleteAsync(Employee employee);
    Task DeleteAsync(IEnumerable<Employee> employees);
}