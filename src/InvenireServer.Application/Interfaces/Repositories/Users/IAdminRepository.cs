using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Interfaces.Repositories.Users;

public interface IAdminRepository : IRepositoryBase<Admin>
{
    Task<Admin?> GetAsync(Jwt jwt);
    Task<EntityDto?> GetAndProjectAsync<EntityDto>(Jwt jwt, Expression<Func<Admin, EntityDto>> selector);
    Task<IEnumerable<Admin>> IndexInactiveAsync();
}