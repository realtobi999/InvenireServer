using InvenireServer.Domain.Entities.Users;
using InvenireServer.Application.Interfaces.Repositories.Users;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Users;

public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(InvenireServerContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Employee>> IndexInactiveAsync()
    {
        var threshold = DateTimeOffset.UtcNow.Add(-Employee.INACTIVE_THRESHOLD);
        return await IndexAsync(e => !e.IsVerified && e.CreatedAt <= threshold);
    }
}