using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InvenireServer.Application.Core.Admins.Commands.Login;

public record LoginAdminCommand : IRequest<LoginAdminCommandResult>
{
    [Required]
    [EmailAddress]
    [JsonPropertyName("email_address")]
    public required string EmailAddress { get; init; }

    [Required]
    [JsonPropertyName("password")]
    public required string Password { get; init; }
}