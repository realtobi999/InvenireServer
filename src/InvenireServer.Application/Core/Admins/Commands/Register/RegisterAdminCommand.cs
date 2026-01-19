using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;

namespace InvenireServer.Application.Core.Admins.Commands.Register;

/// <summary>
/// Represents a request to register an admin.
/// </summary>
[JsonRequest]
public record RegisterAdminCommand : IRequest<RegisterAdminCommandResult>
{
    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [JsonPropertyName("first_name")]
    public required string FirstName { get; init; }

    [JsonPropertyName("last_name")]
    public required string LastName { get; init; }

    [JsonPropertyName("email_address")]
    public required string EmailAddress { get; init; }

    [JsonPropertyName("password")]
    public required string Password { get; init; }

    [JsonPropertyName("password_confirm")]
    public required string PasswordConfirm { get; init; }
}