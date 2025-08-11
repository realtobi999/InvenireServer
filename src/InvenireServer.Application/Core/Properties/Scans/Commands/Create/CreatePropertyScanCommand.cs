using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Scans.Commands.Create;

[JsonRequest]
public record CreatePropertyScanCommand : IRequest<CreatePropertyScanCommandResult>
{
    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonIgnore]
    public Jwt? Jwt { get; init; }
}
