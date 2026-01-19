using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Commands.Scan;

/// <summary>
/// Represents a request to scan a property item.
/// </summary>
public record ScanPropertyItemCommand : IRequest
{
    public required Jwt Jwt { get; init; }
    public required Guid ItemId { get; init; }
}
