using System.Text.Json.Serialization;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Commands.Update;

/// <summary>
/// Represents a request to update a property.
/// </summary>
public record UpdatePropertyCommand : IRequest
{
    [JsonIgnore]
    public Jwt? Jwt { get; init; }
}
