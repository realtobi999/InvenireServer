using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace InvenireServer.Application.Dtos.Admins;

public class LoginAdminResponseDto
{
    [Required, JsonPropertyName("token")]
    public required string Token { get; init; }
}
