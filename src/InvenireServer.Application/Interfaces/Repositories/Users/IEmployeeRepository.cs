using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Common.Queries;

namespace InvenireServer.Application.Interfaces.Repositories.Users;

public interface IEmployeeRepository : IRepositoryBase<Employee>
{
    Task<TResult?> GetAsync<TResult>(Jwt jwt, QueryOptions<Employee, TResult> options);
    Task<Employee?> GetAsync(Jwt jwt);
    Task<IEnumerable<Employee>> IndexInactiveAsync();
}
