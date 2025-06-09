using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InvenireServer.Application.Dtos.Admins;

public class LoginAdminResponseDto
{
    [Required]
    [JsonPropertyName("token")]
    public required string Token { get; init; }
}