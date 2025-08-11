using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Core.Properties.Scans.Commands.Create;

public record CreatePropertyScanCommandResult
{
    public required PropertyScan Scan { get; init; }
}
