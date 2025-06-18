using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using InvenireServer.Domain.Entities;

namespace InvenireServer.Application.Dtos.Organizations;

public record CreateOrganizationInvitationDto
{
    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [JsonPropertyName("description")]
    [MaxLength(OrganizationInvitation.MAX_DESCRIPTION_LENGTH)]
    public string? Description { get; set; }
}