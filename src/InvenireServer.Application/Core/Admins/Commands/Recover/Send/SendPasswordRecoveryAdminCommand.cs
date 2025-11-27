using System.Text.Json.Serialization;

namespace InvenireServer.Application.Core.Admins.Commands.Recover.Send;

public record SendPasswordRecoveryAdminCommand : IRequest
{
    [JsonPropertyName("email_address")]
    public required string EmailAddress { get; init; }

    [JsonIgnore]
    public string? FrontendBaseAddress { get; init; }
}
