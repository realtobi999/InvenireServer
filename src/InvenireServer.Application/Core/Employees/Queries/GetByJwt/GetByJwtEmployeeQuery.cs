using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Employees.Queries.GetByJwt;

public record GetByJwtEmployeeQuery : IRequest<EmployeeDto>
{
    public required Jwt Jwt { get; init; }
}
