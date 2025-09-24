using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Queries.IndexByScan;

public class IndexByScanPropertyItemQuery : IRequest<IndexByScanPropertyItemQueryResponse>
{
    public required Jwt Jwt { get; init; }
    public required Guid ScanId { get; init; }
    public required IndexByScanPropertyItemQueryParameters Parameters { get; set; }
}
