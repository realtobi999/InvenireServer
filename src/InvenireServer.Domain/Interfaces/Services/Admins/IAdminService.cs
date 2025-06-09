using System.Linq.Expressions;
using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Domain.Interfaces.Services.Admins;

public interface IAdminService
{
    Task<Admin> GetAsync(Jwt jwt);
    Task<Admin> GetAsync(Expression<Func<Admin, bool>> predicate);
    Task CreateAsync(Admin admin);
    Jwt CreateJwt(Admin admin);
    Task UpdateAsync(Admin admin);
    Task SendEmailVerificationAsync(Admin admin);
    Task ConfirmEmailVerificationAsync(Admin admin);
}