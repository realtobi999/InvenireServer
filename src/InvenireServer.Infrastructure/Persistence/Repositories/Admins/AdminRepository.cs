using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Interfaces.Repositories.Admins;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Admins;

public class AdminRepository : RepositoryBase<Admin>, IAdminRepository
{
    public AdminRepository(InvenireServerContext context) : base(context)
    {
    }
}