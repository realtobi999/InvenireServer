using InvenireServer.Domain.Core.Entities;
using InvenireServer.Domain.Core.Interfaces.Repositories;

namespace InvenireServer.Infrastructure.Persistence.Repositories;

public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(InvenireServerContext context) : base(context)
    {
    }
}
