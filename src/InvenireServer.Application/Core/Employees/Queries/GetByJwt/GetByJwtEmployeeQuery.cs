using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Employees.Queries.GetByJwt;

/// <summary>
/// Represents a query to get an employee using a JWT.
/// </summary>
public record GetByJwtEmployeeQuery : IRequest<EmployeeDto>
{
    public required Jwt Jwt { get; init; }
}
