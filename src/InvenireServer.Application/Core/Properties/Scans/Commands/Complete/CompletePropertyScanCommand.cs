using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Scans.Commands.Complete;

public class CompletePropertyScanCommand : IRequest
{
    public required Jwt Jwt { get; set; }
}
