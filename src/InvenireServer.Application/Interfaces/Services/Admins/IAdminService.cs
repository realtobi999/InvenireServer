using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Interfaces.Services.Admins;

public interface IAdminService
{
    public IAdminDtoService Dto { get; }
    Task<IEnumerable<Admin>> IndexInactiveAsync();
    Task<Admin> GetAsync(Jwt jwt);
    Task<Admin> GetAsync(Expression<Func<Admin, bool>> predicate);
    Task CreateAsync(Admin admin);
    Task UpdateAsync(Admin admin);
    Task DeleteAsync(Admin admin);
    Task DeleteAsync(IEnumerable<Admin> admins);
}

public interface IAdminDtoService
{
    Task<AdminDto> GetAsync(Jwt jwt);
    Task<AdminDto> GetAsync(Expression<Func<Admin, bool>> predicate);
}