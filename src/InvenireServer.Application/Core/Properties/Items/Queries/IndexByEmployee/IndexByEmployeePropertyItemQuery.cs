using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Queries.IndexByEmployee;

public record IndexByEmployeePropertyItemQuery : IRequest<IndexByEmployeePropertyItemQueryResponse>
{
    public required Jwt Jwt { get; init; }
    public required IndexByEmployeePropertyItemQueryParameters Parameters { get; init; }
}
