using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Scans.Queries.GetActive.ByEmployee;

public class GetByEmployeeActivePropertyScanQuery : IRequest<PropertyScanDto>
{
    public required Jwt Jwt { get; init; }
}