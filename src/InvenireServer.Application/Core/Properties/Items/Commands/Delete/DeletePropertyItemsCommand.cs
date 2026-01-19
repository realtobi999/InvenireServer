using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Commands.Delete;

/// <summary>
/// Represents a request to delete property items.
/// </summary>
[JsonRequest]
public record DeletePropertyItemsCommand : IRequest
{
    [JsonPropertyName("ids")]
    public required List<Guid> Ids { get; init; }

    [JsonIgnore]
    public Jwt? Jwt { get; init; }
}