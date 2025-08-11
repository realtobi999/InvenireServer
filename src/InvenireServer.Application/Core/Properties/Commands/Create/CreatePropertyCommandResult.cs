using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Core.Properties.Commands.Create;

public record CreatePropertyCommandResult
{
    public required Property Property { get; init; }
}