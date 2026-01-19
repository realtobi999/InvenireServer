using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Commands.Delete;

/// <summary>
/// Represents a request to delete a property.
/// </summary>
public record DeletePropertyCommand : IRequest
{
    public required Jwt Jwt { get; init; }
}
