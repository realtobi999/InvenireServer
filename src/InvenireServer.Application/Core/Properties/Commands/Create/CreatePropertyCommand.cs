using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Commands.Create;

/// <summary>
/// Represents a request to create a property.
/// </summary>
[JsonRequest]
public record CreatePropertyCommand : IRequest<CreatePropertyCommandResult>
{
    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [JsonIgnore]
    public Jwt? Jwt { get; init; }
}