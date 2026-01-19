using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Application.Core.Properties.Items.Commands.Update;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands;

/// <summary>
/// Represents payload data for property suggestions.
/// </summary>
[JsonRequest]
public record PropertySuggestionPayload
{
    [JsonPropertyName("delete_commands")]
    public required List<Guid> DeleteCommands { get; set; } = [];

    [JsonPropertyName("create_commands")]
    public required List<CreatePropertyItemCommand> CreateCommands { get; set; } = [];

    [JsonPropertyName("update_commands")]
    public required List<UpdatePropertyItemCommand> UpdateCommands { get; set; } = [];
}
