using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Dtos.Organizations;

public record CreateOrganizationDto
{
    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [Required]
    [JsonPropertyName("name")]
    [MaxLength(Organization.MAX_NAME_LENGTH)]
    public required string Name { get; set; }
}