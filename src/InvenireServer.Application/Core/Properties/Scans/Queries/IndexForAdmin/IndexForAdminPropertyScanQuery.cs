using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Scans.Queries.IndexForAdmin;

public record IndexForAdminPropertyScanQuery : IRequest<IndexForAdminPropertyScanQueryResponse>
{
    public required Jwt Jwt { get; set; }
    public required PaginationParameters Pagination { get; set; }
}
