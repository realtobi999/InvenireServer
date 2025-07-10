using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Commands.Create;

[JsonRequest]
public record CreatePropertyCommand : IRequest<CreatePropertyCommandResponse>
{
    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [JsonIgnore]
    public Jwt? Jwt { get; init; }
}