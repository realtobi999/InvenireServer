using InvenireServer.Domain.Core.Entities;
using InvenireServer.Domain.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Repositories;

public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(InvenireServerContext context) : base(context)
    {
    }

    public async Task<bool> HasUniqueEmailAddressAsync(Employee employee)
    {
        return !await Context.Employees.AnyAsync(e => e.Id != employee.Id && e.EmailAddress == employee.EmailAddress);
    }
}
