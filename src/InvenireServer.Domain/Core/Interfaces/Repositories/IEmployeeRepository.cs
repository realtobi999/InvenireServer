using InvenireServer.Domain.Core.Entities;

namespace InvenireServer.Domain.Core.Interfaces.Repositories;

public interface IEmployeeRepository : IBaseRepository<Employee>
{
    Task<bool> IsEmailAddressUnique(string email);
}
