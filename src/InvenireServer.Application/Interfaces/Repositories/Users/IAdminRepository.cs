using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Interfaces.Repositories.Users;

public interface IAdminRepository : IRepositoryBase<Admin>
{
    IAdminDtoRepository Dto { get; }
    Task<IEnumerable<Admin>> IndexInactiveAsync();
}

public interface IAdminDtoRepository
{
    Task<AdminDto?> GetAsync(Expression<Func<Admin, bool>> predicate);
}
