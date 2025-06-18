using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Interfaces.Repositories.Users;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Users;

public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(InvenireServerContext context) : base(context)
    {
    }
}