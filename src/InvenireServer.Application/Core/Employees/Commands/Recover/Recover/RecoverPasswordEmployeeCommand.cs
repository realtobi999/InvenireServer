using System.Text.Json.Serialization;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Employees.Commands.Recover.Recover;

/// <summary>
/// Represents a request to recover a password for an employee.
/// </summary>
public record RecoverPasswordEmployeeCommand : IRequest
{
    [JsonPropertyName("new_password")]
    public required string NewPassword { get; init; }

    [JsonPropertyName("new_password_confirm")]
    public required string NewPasswordConfirm { get; init; }

    [JsonIgnore]
    public Jwt? Jwt { get; init; }
}
