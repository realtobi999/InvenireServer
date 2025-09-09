using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Core.Properties.Items.Queries.IndexByAdmin;

public record IndexByAdminPropertyItemQuery : IRequest<IndexByAdminPropertyItemQueryResponse>
{
    public required Jwt Jwt { get; init; }
    public required IndexByAdminPropertyItemQueryParameters Parameters { get; init; }
}
