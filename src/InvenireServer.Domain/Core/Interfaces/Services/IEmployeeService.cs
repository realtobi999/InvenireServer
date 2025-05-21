using System.Linq.Expressions;
using InvenireServer.Domain.Core.Entities;

namespace InvenireServer.Domain.Core.Interfaces.Services;

public interface IEmployeeService
{
    Task<Employee> GetAsync(Expression<Func<Employee, bool>> predicate);
    Task CreateAsync(Employee employee);
}
