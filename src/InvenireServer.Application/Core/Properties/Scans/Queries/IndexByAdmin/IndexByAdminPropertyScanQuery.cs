using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Common.Queries;

namespace InvenireServer.Application.Core.Properties.Scans.Queries.IndexByAdmin;

public record IndexByAdminPropertyScanQuery : IRequest<IndexByAdminPropertyScanQueryResponse>
{
    public required Jwt Jwt { get; set; }
    public required QueryPaginationOptions Pagination { get; set; }
}
