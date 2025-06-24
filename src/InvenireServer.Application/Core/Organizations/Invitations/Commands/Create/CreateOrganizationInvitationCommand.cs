using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;

public record CreateOrganizationInvitationCommand : IRequest<CreateOrganizationInvitationCommandResult>
{
    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [JsonPropertyName("description")]
    [MaxLength(OrganizationInvitation.MAX_DESCRIPTION_LENGTH)]
    public string? Description { get; set; }

    [Required]
    [JsonPropertyName("employee_id")]
    public required Guid EmployeeId { get; set; }

    [JsonIgnore]
    public Jwt? Jwt { get; set; }

    [JsonIgnore]
    public Guid? OrganizationId { get; set; }
}