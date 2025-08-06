using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Interfaces.Repositories.Users;

public interface IEmployeeRepository : IRepositoryBase<Employee>
{
    Task<Employee?> GetAsync(Jwt jwt);
    Task<IEnumerable<Employee>> IndexInactiveAsync();
}
