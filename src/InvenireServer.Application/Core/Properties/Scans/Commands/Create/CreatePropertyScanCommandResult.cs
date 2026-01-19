using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Core.Properties.Scans.Commands.Create;

/// <summary>
/// Represents the result of creating a property scan.
/// </summary>
public record CreatePropertyScanCommandResult
{
    public required PropertyScan Scan { get; init; }
}
