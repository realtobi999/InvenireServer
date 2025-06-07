using System.Linq.Expressions;
using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Domain.Interfaces.Services.Employees;

public interface IEmployeeService
{
    Task<Employee> GetAsync(Jwt jwt);

    Task<Employee> GetAsync(Expression<Func<Employee, bool>> predicate);

    Task CreateAsync(Employee employee);

    Jwt CreateJwt(Employee employee);

    Task UpdateAsync(Employee employee);

    Task SendEmailVerificationAsync(Employee employee);

    Task ConfirmEmailVerificationAsync(Employee employee);
}