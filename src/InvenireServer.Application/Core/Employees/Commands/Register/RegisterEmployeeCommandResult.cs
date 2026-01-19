using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Core.Employees.Commands.Register;

/// <summary>
/// Represents the result of registering an employee.
/// </summary>
public record RegisterEmployeeCommandResult
{
    public required Jwt Token { get; init; }
    public required Employee Employee { get; init; }
    public required string TokenString { get; init; }
}