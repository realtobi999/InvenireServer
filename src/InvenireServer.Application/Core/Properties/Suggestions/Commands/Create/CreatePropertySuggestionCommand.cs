using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Application.Core.Properties.Items.Commands.Update;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Create;

[JsonRequest]
public record CreatePropertySuggestionCommand : IRequest<CreatePropertySuggestionCommandResult>
{
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("body")]
    public required RequestBody Body { get; set; }

    [JsonRequest]
    public record RequestBody
    {
        [JsonPropertyName("delete_commands")]
        public required List<Guid> DeleteCommands { get; set; } = [];

        [JsonPropertyName("create_commands")]
        public required List<CreatePropertyItemCommand> CreateCommands { get; set; } = [];

        [JsonPropertyName("update_commands")]
        public required List<UpdatePropertyItemCommand> UpdateCommands { get; set; } = [];
    }

    [JsonIgnore]
    public Jwt? Jwt { get; set; }
}
