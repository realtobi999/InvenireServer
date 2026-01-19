using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Scans.Queries.GetActive.ByEmployee;

/// <summary>
/// Represents a query to get an active property scan for an employee.
/// </summary>
public class GetByEmployeeActivePropertyScanQuery : IRequest<PropertyScanDto>
{
    public required Jwt Jwt { get; init; }
}