using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Commands.Scan;

public record ScanPropertyItemCommand : IRequest
{
    public required Jwt Jwt { get; init; }
    public required Guid ItemId { get; init; }
}
