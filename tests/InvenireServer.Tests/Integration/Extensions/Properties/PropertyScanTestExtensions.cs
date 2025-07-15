using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Core.Properties.Scans.Commands.Create;

namespace InvenireServer.Tests.Integration.Extensions.Properties;

public static class PropertyScanTestExtensions
{
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
