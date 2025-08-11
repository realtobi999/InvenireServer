using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;

namespace InvenireServer.Application.Core.Employees.Commands.Login;

[JsonRequest]
public record LoginEmployeeCommand : IRequest<LoginEmployeeCommandResponse>
{
    [JsonPropertyName("email_address")]
    public required string EmailAddress { get; init; }

    [JsonPropertyName("password")]
    public required string Password { get; init; }
}