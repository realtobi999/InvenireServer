using System.Text.Json.Serialization;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Commands.GenerateCodes;

/// <summary>
/// Represents a request to generate codes for property items.
/// </summary>
public record GenerateCodesPropertyItemsCommand : IRequest<Stream>
{
    [JsonPropertyName("ids")]
    public required List<Guid> Ids { get; init; }

    [JsonIgnore]
    public int Size { get; init; }

    [JsonIgnore]
    public Jwt? Jwt { get; init; }
}
