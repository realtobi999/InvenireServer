using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Interfaces.Repositories;

namespace InvenireServer.Infrastructure.Persistence.Repositories;

public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(InvenireServerContext context) : base(context)
    {
    }
}