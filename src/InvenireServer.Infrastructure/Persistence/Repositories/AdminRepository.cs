using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Interfaces.Repositories;

namespace InvenireServer.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository for accessing and managing <see cref="Admin"/> entities.
/// </summary>
public class AdminRepository : BaseRepository<Admin>, IAdminRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdminRepository"/> class.
    /// </summary>
    /// <param name="context">The database context used for admin operations.</param>
    public AdminRepository(InvenireServerContext context) : base(context)
    {
    }
}
