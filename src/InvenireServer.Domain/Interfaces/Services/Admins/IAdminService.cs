using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Domain.Interfaces.Services.Admins;

public interface IAdminService
{
    Task<Admin> GetAsync(Jwt jwt);
    Task<Admin> GetAsync(Expression<Func<Admin, bool>> predicate);
    Task CreateAsync(Admin admin);
    Jwt CreateJwt(Admin admin);
    Task UpdateAsync(Admin admin);
    Task SendVerificationEmailAsync(Admin admin);
    Task SendOrganizationCreationEmail(Admin admin, Organization organization);
    Task ConfirmEmailVerificationAsync(Admin admin);
}