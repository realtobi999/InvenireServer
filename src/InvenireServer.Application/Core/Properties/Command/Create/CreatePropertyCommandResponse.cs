using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Core.Properties.Command.Create;

public record CreatePropertyCommandResponse
{
    public required Property Property { get; set; }
}
