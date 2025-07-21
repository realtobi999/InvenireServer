using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Scans.Commands.Update;

[JsonRequest]
public record UpdatePropertyScanCommand : IRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonIgnore]
    public Jwt? Jwt { get; set; }
}