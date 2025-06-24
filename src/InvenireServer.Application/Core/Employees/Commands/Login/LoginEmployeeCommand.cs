using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InvenireServer.Application.Core.Employees.Commands.Login;

public record LoginEmployeeCommand : IRequest<LoginEmployeeCommandResult>
{
    [Required]
    [JsonPropertyName("email_address")]
    [EmailAddress]
    public required string EmailAddress { get; init; }

    [Required]
    [JsonPropertyName("password")]
    public required string Password { get; init; }
}