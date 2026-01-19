using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Queries.IndexByEmployee;

/// <summary>
/// Represents a query to index property items for an employee.
/// </summary>
public record IndexByEmployeePropertyItemQuery : IRequest<IndexByEmployeePropertyItemQueryResponse>
{
    public required Jwt Jwt { get; init; }
    public required IndexByEmployeePropertyItemQueryParameters Parameters { get; init; }
}
