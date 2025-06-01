using InvenireServer.Domain.Core.Entities;
using InvenireServer.Domain.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

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

    /// <summary>
    /// Checks whether the specified employee has a unique email address among all other employees.
    /// </summary>
    /// <param name="employee">The employee to check for uniqueness.</param>
    /// <returns><c>true</c> if the email address is unique; otherwise, <c>false</c>.</returns>
    public async Task<bool> HasUniqueEmailAddressAsync(Employee employee)
    {
        return !await Context.Employees.AnyAsync(e => e.Id != employee.Id && e.EmailAddress == employee.EmailAddress);
    }
}
