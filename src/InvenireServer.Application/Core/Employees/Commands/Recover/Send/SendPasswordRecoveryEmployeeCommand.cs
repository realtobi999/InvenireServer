using System.Text.Json.Serialization;

namespace InvenireServer.Application.Core.Employees.Commands.Recover.Send;

/// <summary>
/// Represents a request to send a password recovery for an employee.
/// </summary>
public record SendPasswordRecoveryEmployeeCommand : IRequest
{
    [JsonPropertyName("email_address")]
    public required string EmailAddress { get; init; }

    [JsonIgnore]
    public string? FrontendBaseAddress { get; init; }
}
