using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Common.Queries;

namespace InvenireServer.Application.Core.Properties.Scans.Queries.IndexByAdmin;

public class IndexByAdminPropertyScanQueryHandler : IRequestHandler<IndexByAdminPropertyScanQuery, IndexByAdminPropertyScanQueryResponse>
{
    private readonly IRepositoryManager _repositories;

    public IndexByAdminPropertyScanQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<IndexByAdminPropertyScanQueryResponse> Handle(IndexByAdminPropertyScanQuery request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        var query = new QueryOptions<PropertyScan, PropertyScanDto>
        {
            Selector = PropertyScanDto.IndexByAdminSelector,
            Filtering = new QueryFilteringOptions<PropertyScan>
            {
                Filters =
                [
                    // Core Filter.
                    s => s.PropertyId == property.Id,

                    // Additional Filters.
                    request.Parameters.Active is not null ? s => request.Parameters.Active == (s.CompletedAt == null) : null
                ]
            },
            Pagination = new QueryPaginationOptions(request.Parameters.Limit, request.Parameters.Offset)
        };

        return new IndexByAdminPropertyScanQueryResponse
        {
            Data = [.. await _repositories.Properties.Scans.IndexAsync(query)],
            Limit = request.Parameters.Limit,
            Offset = request.Parameters.Offset,
            TotalCount = await _repositories.Properties.Scans.CountAsync(query.Filtering.Filters!)
        };
    }
}
