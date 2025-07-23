using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Domain.Interfaces.Services.Admins;

public interface IAdminService
{
    Task<IEnumerable<Admin>> IndexInactiveAsync();
    Task<Admin> GetAsync(Jwt jwt);
    Task<Admin> GetAsync(Expression<Func<Admin, bool>> predicate);
    Task CreateAsync(Admin admin);
    Task UpdateAsync(Admin admin);
    Task DeleteAsync(Admin admin);
    Task DeleteAsync(IEnumerable<Admin> admins);
}