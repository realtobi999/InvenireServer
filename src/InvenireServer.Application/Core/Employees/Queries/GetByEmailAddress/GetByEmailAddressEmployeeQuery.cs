using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Employees.Queries.GetByEmailAddress;

/// <summary>
/// Represents a query to get an employee by email address.
/// </summary>
public class GetByEmailAddressEmployeeQuery : IRequest<EmployeeDto>
{
    public required Jwt Jwt { get; init; }
    public required string EmployeeEmailAddress { get; init; }
}
