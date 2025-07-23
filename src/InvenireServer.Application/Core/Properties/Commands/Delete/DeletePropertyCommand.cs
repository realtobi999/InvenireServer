using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Commands.Delete;

public record DeletePropertyCommand : IRequest
{
    public required Jwt Jwt { get; set; }
}
