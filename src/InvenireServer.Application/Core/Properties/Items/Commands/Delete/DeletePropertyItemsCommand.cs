using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Commands.Delete;

[JsonRequest]
public record DeletePropertyItemsCommand : IRequest
{
    [JsonPropertyName("ids")]
    public required List<Guid> Ids { get; init; }

    [JsonIgnore]
    public Jwt? Jwt { get; init; }
}