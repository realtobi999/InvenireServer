using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Interfaces.Repositories.Users;

public interface IAdminRepository : IRepositoryBase<Admin>
{
    Task<Admin?> GetAsync(Jwt jwt);
    Task<TResult?> GetAsync<TResult>(Jwt jwt, QueryOptions<Admin, TResult> options);
    Task<IEnumerable<Admin>> IndexInactiveAsync();
}