using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Scans.Commands.Complete;

public record CompletePropertyScanCommand : IRequest
{
    public required Jwt Jwt { get; init; }
}
