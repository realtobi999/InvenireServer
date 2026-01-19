using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Scans.Commands.Complete;

/// <summary>
/// Represents a request to complete a property scan.
/// </summary>
public record CompletePropertyScanCommand : IRequest
{
    public required Jwt Jwt { get; init; }
}
