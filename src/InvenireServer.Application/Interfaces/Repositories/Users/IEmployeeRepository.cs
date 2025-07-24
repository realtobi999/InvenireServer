using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Interfaces.Repositories.Users;

public interface IEmployeeRepository : IRepositoryBase<Employee>
{
    IEmployeeDtoRepository Dto { get; }
    Task<IEnumerable<Employee>> IndexInactiveAsync();
}

public interface IEmployeeDtoRepository
{
    Task<EmployeeDto?> GetAsync(Expression<Func<Employee, bool>> predicate);
}