using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Application.Dtos.Employees;

namespace InvenireServer.Application.Core.Employees.Queries.GetById;

public record GetByIdEmployeeQueryResponse
{
    public required EmployeeDto EmployeeDto { get; set; }
}
