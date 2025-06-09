using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Domain.Entities;

namespace InvenireServer.Application.Interfaces.Factories.Employees;

public interface IEmployeeFactory
{
    Employee Create(RegisterEmployeeDto dto);
}