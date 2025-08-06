using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;

namespace InvenireServer.Application.Core.Employees.Commands.Login;

[JsonResponse]
public class LoginEmployeeCommandResponse
{
    [JsonPropertyName("token")]
    public required string Token { get; init; }
}