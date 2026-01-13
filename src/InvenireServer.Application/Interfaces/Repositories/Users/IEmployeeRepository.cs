using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Common.Queries;

namespace InvenireServer.Application.Interfaces.Repositories.Users;

/// <summary>
/// Defines a repository for employee users.
/// </summary>
public interface IEmployeeRepository : IRepositoryBase<Employee>
{
    /// <summary>
    /// Gets an employee using the JWT claims.
    /// </summary>
    /// <param name="jwt">JWT containing the employee identifier claim.</param>
    /// <returns>Awaitable task returning the employee or null.</returns>
    Task<Employee?> GetAsync(Jwt jwt);

    /// <summary>
    /// Gets a projected employee using the JWT claims and query options.
    /// </summary>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <param name="jwt">JWT containing the employee identifier claim.</param>
    /// <param name="options">Query options.</param>
    /// <returns>Awaitable task returning the employee or null.</returns>
    Task<TResult?> GetAsync<TResult>(Jwt jwt, QueryOptions<Employee, TResult> options);

    /// <summary>
    /// Returns employees that are inactive.
    /// </summary>
    /// <returns>Awaitable task returning inactive employees.</returns>
    Task<IEnumerable<Employee>> IndexInactiveAsync();
}
