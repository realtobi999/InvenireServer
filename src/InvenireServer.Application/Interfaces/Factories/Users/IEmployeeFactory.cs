using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Interfaces.Factories.Users;

public interface IEmployeeFactory
{
    Employee Create(RegisterEmployeeDto dto);
}