using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Interfaces.Repositories.Users;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Users;

public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(InvenireServerContext context) : base(context)
    {
        Dto = new EmployeeDtoRepository(context);
    }

    public IEmployeeDtoRepository Dto { get; }

    public async Task<IEnumerable<Employee>> IndexInactiveAsync()
    {
        var threshold = DateTimeOffset.UtcNow.Add(-Employee.INACTIVE_THRESHOLD);
        return await IndexAsync(e => !e.IsVerified && e.CreatedAt <= threshold);
    }
}

public class EmployeeDtoRepository : IEmployeeDtoRepository
{
    private readonly InvenireServerContext _context;

    public EmployeeDtoRepository(InvenireServerContext context)
    {
        _context = context;
    }

    public async Task<EmployeeDto?> GetAsync(Expression<Func<Employee, bool>> predicate)
    {
        return await _context.Set<Employee>()
            .AsNoTracking()
            .Where(predicate)
            .Select(e => EmployeeDto.FromEmployee(e))
            .FirstOrDefaultAsync();
    }
}
