using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Interfaces.Repositories.Users;
using InvenireServer.Domain.Entities.Common.Queries;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Users;

/// <summary>
/// Default implementation of <see cref="IAdminRepository"/>.
/// </summary>
public class AdminRepository : RepositoryBase<Admin>, IAdminRepository
{
    public AdminRepository(InvenireServerContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets an admin using the JWT claims.
    /// </summary>
    /// <param name="jwt">JWT containing the admin identifier claim.</param>
    /// <returns>Awaitable task returning the admin or null.</returns>
    public async Task<Admin?> GetAsync(Jwt jwt)
    {
        var claim = jwt.Payload.FirstOrDefault(c => c.Type == "admin_id" && !string.IsNullOrWhiteSpace(c.Value));
        if (claim is null) throw new BadRequest400Exception("Missing or invalid 'admin_id' claim.");

        if (!Guid.TryParse(claim.Value, out var id)) throw new BadRequest400Exception("Invalid format for 'admin_id' claim.");

        return await GetAsync(e => e.Id == id);
    }

    /// <summary>
    /// Gets a projected admin using the JWT claims and query options.
    /// </summary>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <param name="jwt">JWT containing the admin identifier claim.</param>
    /// <param name="options">Query options.</param>
    /// <returns>Awaitable task returning the admin or null.</returns>
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

    /// <summary>
    /// Returns admins that are inactive.
    /// </summary>
    /// <returns>Awaitable task returning inactive admins.</returns>
    public async Task<IEnumerable<Admin>> IndexInactiveAsync()
    {
        var threshold = DateTimeOffset.UtcNow.Add(-Admin.INACTIVE_THRESHOLD);
        return await IndexAsync(e => !e.IsVerified && e.CreatedAt <= threshold);
    }

    /// <summary>
    /// Updates an admin entity in the repository.
    /// </summary>
    /// <param name="admin">Admin to update.</param>
    public override void Update(Admin admin)
    {
        admin.LastUpdatedAt = DateTimeOffset.UtcNow;
        base.Update(admin);
    }
}
