using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Employees.Commands.Login;

/// <summary>
/// Represents the result of authenticating an employee.
/// </summary>
public class LoginEmployeeCommandResult
{
    public required Jwt Token { get; init; }
    public required string TokenString { get; init; }
}


