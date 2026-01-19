using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Scans.Commands.Update;

/// <summary>
/// Represents a request to update a property scan.
/// </summary>
[JsonRequest]
public record UpdatePropertyScanCommand : IRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonIgnore]
    public Jwt? Jwt { get; init; }
}