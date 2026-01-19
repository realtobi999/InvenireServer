using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Admins.Commands.Update;

/// <summary>
/// Represents a request to update an admin.
/// </summary>
[JsonRequest]
public record UpdateAdminCommand : IRequest
{
    [JsonPropertyName("first_name")]
    public required string FirstName { get; init; }

    [JsonPropertyName("last_name")]
    public required string LastName { get; init; }

    [JsonIgnore]
    public Jwt? Jwt { get; init; }
}
