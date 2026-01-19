using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Core.Properties.Commands.Create;

/// <summary>
/// Represents the result of creating a property.
/// </summary>
public record CreatePropertyCommandResult
{
    public required Property Property { get; init; }
}