using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Application.Interfaces.Repositories.Users;
using InvenireServer.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Users;

public class AdminRepository : RepositoryBase<Admin>, IAdminRepository
{
    public AdminRepository(InvenireServerContext context) : base(context)
    {
        Dto = new AdminDtoRepository(context);
    }

    public IAdminDtoRepository Dto { get; }

    public async Task<IEnumerable<Admin>> IndexInactiveAsync()
    {
        var threshold = DateTimeOffset.UtcNow.Add(-Admin.INACTIVE_THRESHOLD);
        return await IndexAsync(e => !e.IsVerified && e.CreatedAt <= threshold);
    }
}

public class AdminDtoRepository : IAdminDtoRepository
{
    private readonly InvenireServerContext _context;

    public AdminDtoRepository(InvenireServerContext context)
    {
        _context = context;
    }

    public async Task<AdminDto?> GetAsync(Expression<Func<Admin, bool>> predicate)
    {
        return await _context.Set<Admin>()
            .AsNoTracking()
            .Where(predicate)
            .Select(AdminDto.FromAdminSelector)
            .FirstOrDefaultAsync();
    }
}
