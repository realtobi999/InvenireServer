using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Domain.Interfaces.Repositories.Users;

public interface IEmployeeRepository : IRepositoryBase<Employee>
{
    Task<IEnumerable<Employee>> IndexInactiveAsync();
}