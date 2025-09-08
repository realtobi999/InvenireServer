using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Interfaces.Repositories.Users;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Users;

public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(InvenireServerContext context) : base(context)
    {
    }

    public async Task<TResult?> GetAsync<TResult>(Jwt jwt, QueryOptions<Employee, TResult> options)
    {
        var claim = jwt.Payload.FirstOrDefault(c => c.Type == "employee_id" && !string.IsNullOrWhiteSpace(c.Value));
        if (claim is null) throw new BadRequest400Exception("Missing or invalid 'employee_id' claim.");

        if (!Guid.TryParse(claim.Value, out var id)) throw new BadRequest400Exception("Invalid format for 'employee_id' claim.");

        return await GetAsync(e => e.Id == id, options);
    }

    public async Task<Employee?> GetAsync(Jwt jwt)
    {
        var claim = jwt.Payload.FirstOrDefault(c => c.Type == "employee_id" && !string.IsNullOrWhiteSpace(c.Value));
        if (claim is null) throw new BadRequest400Exception("Missing or invalid 'employee_id' claim.");

        if (!Guid.TryParse(claim.Value, out var id)) throw new BadRequest400Exception("Invalid format for 'employee_id' claim.");

        return await GetAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Employee>> IndexInactiveAsync()
    {
        var threshold = DateTimeOffset.UtcNow.Add(-Employee.INACTIVE_THRESHOLD);
        return await IndexAsync(e => !e.IsVerified && e.CreatedAt <= threshold);
    }

    public override void Update(Employee employee)
    {
        employee.LastUpdatedAt = DateTimeOffset.UtcNow;
        base.Update(employee);
    }
}