using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Core.Employees.Commands.Register;

public record RegisterEmployeeCommandResult
{
    public required Employee Employee { get; init; }
    public required RegisterEmployeeCommandResponse Response { get; set; }
}

[JsonResponse]
public record RegisterEmployeeCommandResponse
{
    [JsonPropertyName("employee")]
    public required string Token { get; init; }
}