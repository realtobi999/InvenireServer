using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Interfaces.Repositories.Users;
using InvenireServer.Domain.Entities.Common.Queries;
using SQLitePCL;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Users;

/// <summary>
/// Default implementation of <see cref="IEmployeeRepository"/>.
/// </summary>
public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(InvenireServerContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets an employee using the JWT claims.
    /// </summary>
    /// <param name="jwt">JWT containing the employee identifier claim.</param>
    /// <returns>Awaitable task returning the employee or null.</returns>
    public async Task<Employee?> GetAsync(Jwt jwt)
    {
        var claim = jwt.Payload.FirstOrDefault(c => c.Type == "employee_id" && !string.IsNullOrWhiteSpace(c.Value));
        if (claim is null) throw new BadRequest400Exception("Missing or invalid 'employee_id' claim.");

        if (!Guid.TryParse(claim.Value, out var id)) throw new BadRequest400Exception("Invalid format for 'employee_id' claim.");

        return await GetAsync(e => e.Id == id);
    }

    /// <summary>
    /// Gets a projected employee using the JWT claims and query options.
    /// </summary>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <param name="jwt">JWT containing the employee identifier claim.</param>
    /// <param name="options">Query options.</param>
    /// <returns>Awaitable task returning the employee or null.</returns>
    public async Task<TResult?> GetAsync<TResult>(Jwt jwt, QueryOptions<Employee, TResult> options)
    {
        var claim = jwt.Payload.FirstOrDefault(c => c.Type == "employee_id" && !string.IsNullOrWhiteSpace(c.Value));
        if (claim is null) throw new BadRequest400Exception("Missing or invalid 'employee_id' claim.");

        if (!Guid.TryParse(claim.Value, out var id)) throw new BadRequest400Exception("Invalid format for 'employee_id' claim.");

        if (options.Filtering is null)
            options.Filtering = new QueryFilteringOptions<Employee>()
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
    /// Returns employees that are inactive.
    /// </summary>
    /// <returns>Awaitable task returning inactive employees.</returns>
    public async Task<IEnumerable<Employee>> IndexInactiveAsync()
    {
        var threshold = DateTimeOffset.UtcNow.Add(-Employee.INACTIVE_THRESHOLD);
        return await IndexAsync(e => !e.IsVerified && e.CreatedAt <= threshold);
    }

    /// <summary>
    /// Updates an employee entity in the repository.
    /// </summary>
    /// <param name="employee">Employee to update.</param>
    public override void Update(Employee employee)
    {
        employee.LastUpdatedAt = DateTimeOffset.UtcNow;
        base.Update(employee);
    }
}
