using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Interfaces.Repositories.Employees;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Employees;

public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(InvenireServerContext context) : base(context)
    {
    }
}