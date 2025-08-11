using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Entities.Common;
using System.Linq.Expressions;

namespace InvenireServer.Application.Interfaces.Repositories.Users;

public interface IEmployeeRepository : IRepositoryBase<Employee>
{
    Task<Employee?> GetAsync(Jwt jwt);
    Task<EntityDto?> GetAndProjectAsync<EntityDto>(Jwt jwt, Expression<Func<Employee, EntityDto>> selector);
    Task<IEnumerable<Employee>> IndexInactiveAsync();
}
