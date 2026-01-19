using System.Text.Json.Serialization;

namespace InvenireServer.Application.Core.Admins.Commands.Recover.Send;

/// <summary>
/// Represents a request to send a password recovery for an admin.
/// </summary>
public record SendPasswordRecoveryAdminCommand : IRequest
{
    [JsonPropertyName("email_address")]
    public required string EmailAddress { get; init; }

    [JsonIgnore]
    public string? FrontendBaseAddress { get; init; }
}
