using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Update;

/// <summary>
/// Represents a request to update an organization invitation.
/// </summary>
[JsonRequest]
public record UpdateOrganizationInvitationCommand : IRequest
{
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonIgnore]
    public Jwt? Jwt { get; init; }

    [JsonIgnore]
    public Guid? InvitationId { get; init; }
}
