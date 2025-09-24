using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Properties.Items.Queries.IndexByScan;

public class IndexByScanPropertyItemQueryHandler : IRequestHandler<IndexByScanPropertyItemQuery, IndexByScanPropertyItemQueryResponse>
{
    private readonly IRepositoryManager _repositories;

    public IndexByScanPropertyItemQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<IndexByScanPropertyItemQueryResponse> Handle(IndexByScanPropertyItemQuery request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        // Retrieve the scan and project all helper fields of the scanned  items
        // into it. This approach works efficiently  even  with  large  datasets
        // (10K+ items). Optionally filter the projected items based on  whether
        // they are marked as scanned or not.
        var scan = await _repositories.Properties.Scans.GetAsync(new QueryOptions<PropertyScan, PropertyScan>
        {
            Selector = s => new PropertyScan
            {
                Id = s.Id,
                PropertyId = s.PropertyId,
                Name = s.Name,
                Description = s.Description,
                Status = s.Status,
                CreatedAt = s.CreatedAt,
                CompletedAt = s.CompletedAt,
                LastUpdatedAt = s.LastUpdatedAt,
                ScannedItems = request.Parameters.Scanned == null ? s.ScannedItems.ToList() : request.Parameters.Scanned.Value ? s.ScannedItems.Where(si => si.IsScanned).ToList() : s.ScannedItems.Where(si => !si.IsScanned).ToList(),

            },
            Filtering = new QueryFilteringOptions<PropertyScan>
            {
                Filters =
                [
                    s => s.Id == request.ScanId && s.PropertyId == property.Id,
                ]
            },
        }) ?? throw new NotFound404Exception("The scan was not found in the system.");

        var query = new QueryOptions<PropertyItem, PropertyItemDto>
        {
            Selector = PropertyItemDto.CoreSelector,
            Ordering = new QueryOrderingOptions<PropertyItem>(request.Parameters.Order, request.Parameters.Desc),
            Filtering = new QueryFilteringOptions<PropertyItem>
            {
                Filters =
                [
                    // Core Filter.
                    i => scan.ScannedItems.Select(i => i.PropertyItemId).Contains(i.Id),

                    // Search Filter.
                    request.Parameters.SearchQuery is not null ? _repositories.Properties.Items.BuildSearchExpression(request.Parameters.SearchQuery) : null,

                    // Additional Filters.
                    request.Parameters.MaxPrice is not null ? i => i.Price <= request.Parameters.MaxPrice : null,
                    request.Parameters.MinPrice is not null ? i => i.Price >= request.Parameters.MinPrice : null,
                    request.Parameters.DateOfPurchaseFrom is not null ? i => i.DateOfPurchase > request.Parameters.DateOfPurchaseFrom : null,
                    request.Parameters.DateOfPurchaseTo is not null ? i => i.DateOfPurchase < request.Parameters.DateOfPurchaseTo : null,
                    request.Parameters.CreatedAtFrom is not null ? i => i.CreatedAt > request.Parameters.CreatedAtFrom : null,
                    request.Parameters.CreatedAtTo is not null ? i => i.CreatedAt < request.Parameters.CreatedAtTo : null,
                    request.Parameters.EmployeeId is not null ? i => i.EmployeeId == request.Parameters.EmployeeId : null,
                    !string.IsNullOrEmpty(request.Parameters.Room) ? i => i.Location.Room.Contains(request.Parameters.Room) : null,
                    !string.IsNullOrEmpty(request.Parameters.Building) ? i => i.Location.Building.Contains(request.Parameters.Building) : null,
                ]
            },
            Pagination = new QueryPaginationOptions(request.Parameters.Limit, request.Parameters.Offset)
        };
        var items = await _repositories.Properties.Items.IndexAsync(query);

        // Load all the employees.
        foreach (var item in items)
        {
            item.Employee = await _repositories.Employees.GetAsync(new QueryOptions<Employee, EmployeeDto>
            {
                Selector = EmployeeDto.BaseSelector,
                Filtering = new QueryFilteringOptions<Employee>
                {
                    Filters = [e => e.Id == item.EmployeeId]
                },
            });
        }

        return new IndexByScanPropertyItemQueryResponse
        {
            Data = [.. items],
            Limit = request.Parameters.Limit,
            Offset = request.Parameters.Offset,
            TotalCount = await _repositories.Properties.Items.CountAsync(query.Filtering.Filters!)
        };
    }
}
