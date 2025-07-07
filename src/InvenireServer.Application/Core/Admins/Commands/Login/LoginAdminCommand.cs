using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;

namespace InvenireServer.Application.Core.Admins.Commands.Login;

[JsonRequest]
public record LoginAdminCommand : IRequest<LoginAdminCommandResult>
{
    [JsonPropertyName("email_address")]
    public required string EmailAddress { get; init; }

    [JsonPropertyName("password")]
    public required string Password { get; init; }
}