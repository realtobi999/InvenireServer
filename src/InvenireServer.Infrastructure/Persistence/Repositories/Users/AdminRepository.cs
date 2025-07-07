using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Interfaces.Repositories.Users;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Users;

public class AdminRepository : RepositoryBase<Admin>, IAdminRepository
{
    public AdminRepository(InvenireServerContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Admin>> IndexInactiveAsync()
    {
        var threshold = DateTimeOffset.UtcNow.Add(Admin.INACTIVE_THRESHOLD);
        return await IndexAsync(e => !e.IsVerified 
& e.CreatedAt <= threshold);
    }
}