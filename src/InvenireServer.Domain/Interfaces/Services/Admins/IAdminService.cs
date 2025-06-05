using System.Linq.Expressions;
using InvenireServer.Domain.Entities;

namespace InvenireServer.Domain.Interfaces.Services.Admins;

public interface IAdminService
{
    Task<Admin> GetAsync(Expression<Func<Admin, bool>> predicate);
    Task CreateAsync(Admin admin);
}
