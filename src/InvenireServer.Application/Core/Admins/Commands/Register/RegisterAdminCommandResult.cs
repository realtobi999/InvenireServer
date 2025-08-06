using System.Text.Json.Serialization;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Application.Attributes;

namespace InvenireServer.Application.Core.Admins.Commands.Register;

public record RegisterAdminCommandResult
{
    public required Admin Admin { get; init; }
    public required RegisterAdminCommandResponse Response { get; init; }
}

[JsonResponse]
public record RegisterAdminCommandResponse
{
    [JsonPropertyName("token")]
    public required string Token { get; init; }
}