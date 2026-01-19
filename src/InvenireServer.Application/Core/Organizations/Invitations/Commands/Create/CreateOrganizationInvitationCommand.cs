using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;

/// <summary>
/// Represents a request to create an organization invitation.
/// </summary>
[JsonRequest]
public record CreateOrganizationInvitationCommand : IRequest<CreateOrganizationInvitationCommandResult>
{
    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("employee_id")]
    public Guid? EmployeeId { get; init; }

    [JsonPropertyName("employee_email_address")]
    public string? EmployeeEmailAddress { get; set; }

    [JsonIgnore]
    public Jwt? Jwt { get; init; }
}