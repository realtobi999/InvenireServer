using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Domain.Interfaces.Repositories.Users;

public interface IAdminRepository : IRepositoryBase<Admin>
{
    Task<IEnumerable<Admin>> IndexInactiveAsync();
}