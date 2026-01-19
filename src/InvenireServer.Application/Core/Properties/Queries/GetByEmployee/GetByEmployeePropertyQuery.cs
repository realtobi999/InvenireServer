using InvenireServer.Domain.Entities.Common;
using InvenireServer.Application.Dtos.Properties;

namespace InvenireServer.Application.Core.Properties.Queries.GetByEmployee;

/// <summary>
/// Represents a query to get a property for an employee.
/// </summary>
public record GetByEmployeePropertyQuery : IRequest<PropertyDto>
{
    public required Jwt Jwt { get; init; }
}
