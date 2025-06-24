using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Core.Organizations.Commands.Create;

public record CreateOrganizationCommand : IRequest<CreateOrganizationCommandResult>
{
    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [Required]
    [JsonPropertyName("name")]
    [MaxLength(Organization.MAX_NAME_LENGTH)]
    public required string Name { get; set; }

    [JsonIgnore]
    public Jwt? Jwt { get; set; }

    [JsonIgnore]
    public string? FrontendBaseUrl { get; set; }
}