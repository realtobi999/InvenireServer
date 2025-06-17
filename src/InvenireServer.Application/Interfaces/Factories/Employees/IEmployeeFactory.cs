using InvenireServer.Domain.Entities;
using InvenireServer.Application.Dtos.Employees;

namespace InvenireServer.Application.Interfaces.Factories.Employees;

public interface IEmployeeFactory
{
    Employee Create(RegisterEmployeeDto dto);
}