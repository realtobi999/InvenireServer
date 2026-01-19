using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Commands.Update;

/// <summary>
/// Represents a request to update an organization.
/// </summary>
[JsonRequest]
public record UpdateOrganizationCommand : IRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonIgnore]
    public Jwt? Jwt { get; init; }
}
