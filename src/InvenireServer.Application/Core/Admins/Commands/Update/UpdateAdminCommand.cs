using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Admins.Commands.Update;

[JsonRequest]
public record UpdateAdminCommand : IRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonIgnore]
    public Jwt? Jwt { get; set; }
}
