using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Interfaces.Repositories.Users;
using InvenireServer.Domain.Entities.Common.Queries;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Users;

public class AdminRepository : RepositoryBase<Admin>, IAdminRepository
{
    public AdminRepository(InvenireServerContext context) : base(context)
    {
    }

    public async Task<Admin?> GetAsync(Jwt jwt)
    {
        var claim = jwt.Payload.FirstOrDefault(c => c.Type == "admin_id" && !string.IsNullOrWhiteSpace(c.Value));
        if (claim is null) throw new BadRequest400Exception("Missing or invalid 'admin_id' claim.");

        if (!Guid.TryParse(claim.Value, out var id)) throw new BadRequest400Exception("Invalid format for 'admin_id' claim.");

        return await GetAsync(e => e.Id == id);
    }

    public async Task<TResult?> GetAsync<TResult>(Jwt jwt, QueryOptions<Admin, TResult> options)
    {
        var claim = jwt.Payload.FirstOrDefault(c => c.Type == "admin_id" && !string.IsNullOrWhiteSpace(c.Value));
        if (claim is null) throw new BadRequest400Exception("Missing or invalid 'admin_id' claim.");

        if (!Guid.TryParse(claim.Value, out var id)) throw new BadRequest400Exception("Invalid format for 'admin_id' claim.");

        if (options.Filtering is null)
            options.Filtering = new QueryFilteringOptions<Admin>()
            {
                Filters =
                [
                    e => e.Id == id
                ]
            };
        else
            options.Filtering.Filters.Add(e => e.Id == id);


        return await GetAsync(options);
    }

    public async Task<IEnumerable<Admin>> IndexInactiveAsync()
    {
        var threshold = DateTimeOffset.UtcNow.Add(-Admin.INACTIVE_THRESHOLD);
        return await IndexAsync(e => !e.IsVerified && e.CreatedAt <= threshold);
    }

    public override void Update(Admin admin)
    {
        admin.LastUpdatedAt = DateTimeOffset.UtcNow;
        base.Update(admin);
    }
}
