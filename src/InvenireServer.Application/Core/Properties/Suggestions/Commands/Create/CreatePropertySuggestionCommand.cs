using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Application.Core.Properties.Items.Commands.Update;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Create;

public record CreatePropertySuggestionCommand : IRequest<CreatePropertySuggestionCommandResult>
{
    public required Guid? Id { get; set; }

    public required string Name { get; set; }

    public required string? Description { get; set; }

    public required string RequestBody { get; set; }

    public required PropertySuggestionRequestType RequestType { get; set; }

    public required Jwt Jwt { get; set; }
}

[JsonRequest]
public abstract record BasePropertySuggestionRequest
{
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

[JsonRequest]
public record PostPropertySuggestionRequest : BasePropertySuggestionRequest
{
    [JsonPropertyName("request_body")]
    public required List<CreatePropertyItemCommand> RequestBody { get; set; }
}

[JsonRequest]
public record PutPropertySuggestionRequest : BasePropertySuggestionRequest
{
    [JsonPropertyName("request_body")]
    public required List<UpdatePropertyItemCommand> RequestBody { get; set; }
}

[JsonRequest]
public record DeletePropertySuggestionRequest : BasePropertySuggestionRequest
{
    [JsonPropertyName("request_body")]
    public required List<Guid> RequestBody { get; set; }
}