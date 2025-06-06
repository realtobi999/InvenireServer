using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace InvenireServer.Application.Dtos.Admins;

public record LoginAdminDto
{
    [Required, JsonPropertyName("email_address"), EmailAddress]
    public required string EmailAddress { get; init; }

    [Required, JsonPropertyName("password")]
    public required string Password { get; init; }
}
