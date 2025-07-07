using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Commands.Delete;

[JsonRequest]
public record DeletePropertyItemsCommand : IRequest
{
    [Required]
    [MinLength(1)]
    [JsonPropertyName("items")]
    public required List<Guid> ItemIds { get; set; }

    [JsonIgnore]
    public Jwt? Jwt { get; set; }

    [JsonIgnore]
    public Guid? PropertyId { get; set; }

    [JsonIgnore]
    public Guid? OrganizationId { get; set; }
}
