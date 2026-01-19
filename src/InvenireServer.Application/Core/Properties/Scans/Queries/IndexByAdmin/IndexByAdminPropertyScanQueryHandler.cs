using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Scans.Queries.IndexByAdmin;

/// <summary>
/// Handler for the query to index property scans for an admin.
/// </summary>
public class IndexByAdminPropertyScanQueryHandler : IRequestHandler<IndexByAdminPropertyScanQuery, IndexByAdminPropertyScanQueryResponse>
{
    private readonly IRepositoryManager _repositories;

    public IndexByAdminPropertyScanQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the query to index property scans for an admin.
    /// </summary>
    /// <param name="request">Query to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task returning the response.</returns>
    public async Task<IndexByAdminPropertyScanQueryResponse> Handle(IndexByAdminPropertyScanQuery request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        var query = new QueryOptions<PropertyScan, PropertyScanDto>
        {
            Selector = PropertyScanDtoSelector,
            Ordering = new QueryOrderingOptions<PropertyScan>(request.Parameters.Order, request.Parameters.Desc),
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

    private static Expression<Func<PropertyScan, PropertyScanDto>> PropertyScanDtoSelector
    {
        get
        {
            return s => new PropertyScanDto
            {
                Id = s.Id,
                PropertyId = s.PropertyId,
                Name = s.Name,
                Description = s.Description,
                Status = s.Status,
                CreatedAt = s.CreatedAt,
                CompletedAt = s.CompletedAt,
                LastUpdatedAt = s.LastUpdatedAt,
                ScannedItemsSummary = s.ScannedItems.Count == 0 ? null : new PropertyScanDtoScannedItemsSummary
                {
                    TotalItemsToScan = s.ScannedItems.Count,
                    TotalScannedItems = s.ScannedItems.Where(si => si.IsScanned).ToList().Count,
                },
            };
        }
    }
}
