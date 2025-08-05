using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Interfaces.Repositories.Users;

public interface IAdminRepository : IRepositoryBase<Admin>
{
    Task<Admin?> GetAsync(Jwt jwt);
    Task<IEnumerable<Admin>> IndexInactiveAsync();
}