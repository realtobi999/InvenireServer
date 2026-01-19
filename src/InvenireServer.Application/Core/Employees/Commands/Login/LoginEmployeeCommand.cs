using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;

namespace InvenireServer.Application.Core.Employees.Commands.Login;

/// <summary>
/// Represents a request to authenticate an employee.
/// </summary>
[JsonRequest]
public record LoginEmployeeCommand : IRequest<LoginEmployeeCommandResult>
{
    [JsonPropertyName("email_address")]
    public required string EmailAddress { get; init; }

    [JsonPropertyName("password")]
    public required string Password { get; init; }
}