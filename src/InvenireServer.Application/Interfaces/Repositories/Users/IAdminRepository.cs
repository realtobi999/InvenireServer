using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Common.Queries;

namespace InvenireServer.Application.Interfaces.Repositories.Users;

/// <summary>
/// Defines a repository for admin users.
/// </summary>
public interface IAdminRepository : IRepositoryBase<Admin>
{
    /// <summary>
    /// Gets an admin using the JWT claims.
    /// </summary>
    /// <param name="jwt">JWT containing the admin identifier claim.</param>
    /// <returns>Awaitable task returning the admin or null.</returns>
    Task<Admin?> GetAsync(Jwt jwt);

    /// <summary>
    /// Gets a projected admin using the JWT claims and query options.
    /// </summary>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <param name="jwt">JWT containing the admin identifier claim.</param>
    /// <param name="options">Query options.</param>
    /// <returns>Awaitable task returning the admin or null.</returns>
    Task<TResult?> GetAsync<TResult>(Jwt jwt, QueryOptions<Admin, TResult> options);

    /// <summary>
    /// Returns admins that are inactive.
    /// </summary>
    /// <returns>Awaitable task returning inactive admins.</returns>
    Task<IEnumerable<Admin>> IndexInactiveAsync();
}
