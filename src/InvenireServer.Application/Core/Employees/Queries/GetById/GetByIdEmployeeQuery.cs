using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Employees.Queries.GetById;

/// <summary>
/// Represents a query to get an employee by ID.
/// </summary>
public record GetByIdEmployeeQuery : IRequest<EmployeeDto>
{
    public required Jwt Jwt { get; init; }
    public required Guid EmployeeId { get; init; }
}
