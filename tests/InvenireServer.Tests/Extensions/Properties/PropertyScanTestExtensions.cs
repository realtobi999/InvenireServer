using InvenireServer.Application.Core.Properties.Scans.Commands.Create;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Tests.Extensions.Properties;

/// <summary>
/// Provides test extensions for <see cref="PropertyScan"/>.
/// </summary>
public static class PropertyScanTestExtensions
{
    /// <summary>
    /// Creates a <see cref="CreatePropertyScanCommand"/> from a property scan.
    /// </summary>
    /// <param name="scan">Source property scan.</param>
    /// <returns>Create property scan command.</returns>
    public static CreatePropertyScanCommand ToCreatePropertyScanCommand(this PropertyScan scan)
    {
        var dto = new CreatePropertyScanCommand
        {
            Id = scan.Id,
            Name = scan.Name,
            Description = scan.Description
        };

        return dto;
    }
}
