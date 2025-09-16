using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Queries.IndexByAdmin;

public record IndexByAdminPropertyItemQuery : IRequest<IndexByAdminPropertyItemQueryResponse>
{
    public required Jwt Jwt { get; init; }
    public required IndexByAdminPropertyItemQueryParameters Parameters { get; init; }
}
