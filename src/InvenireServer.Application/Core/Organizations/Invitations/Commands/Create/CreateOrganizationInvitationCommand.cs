using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;

[JsonRequest]
public record CreateOrganizationInvitationCommand : IRequest<CreateOrganizationInvitationCommandResult>
{
    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("employee_id")]
    public required Guid EmployeeId { get; set; }

    [JsonIgnore]
    public Jwt? Jwt { get; set; }
}