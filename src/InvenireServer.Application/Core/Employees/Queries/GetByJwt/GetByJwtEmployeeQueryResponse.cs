using InvenireServer.Application.Dtos.Employees;

namespace InvenireServer.Application.Core.Employees.Queries.GetByJwt;

public class GetByJwtEmployeeQueryResponse
{
    public required EmployeeDto EmployeeDto { get; set; }
}
