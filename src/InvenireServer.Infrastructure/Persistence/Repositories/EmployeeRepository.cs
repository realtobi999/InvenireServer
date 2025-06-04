using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Interfaces.Repositories;

namespace InvenireServer.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository for accessing and managing <see cref="Employee"/> entities.
/// </summary>
public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeRepository"/> class.
    /// </summary>
    /// <param name="context">The database context used for employee operations.</param>
    public EmployeeRepository(InvenireServerContext context) : base(context)
    {
    }
}