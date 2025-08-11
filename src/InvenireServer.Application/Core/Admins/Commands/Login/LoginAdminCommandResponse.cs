using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;

namespace InvenireServer.Application.Core.Admins.Commands.Login;

[JsonResponse]
public record LoginAdminCommandResponse
{
    [JsonPropertyName("token")]
    public required string Token { get; init; }
}