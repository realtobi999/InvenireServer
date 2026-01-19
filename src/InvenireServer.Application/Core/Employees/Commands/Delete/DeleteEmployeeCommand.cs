using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Employees.Commands.Delete;

/// <summary>
/// Represents a request to delete an employee.
/// </summary>
public record DeleteEmployeeCommand : IRequest
{
    public required Jwt Jwt { get; init; }
}
