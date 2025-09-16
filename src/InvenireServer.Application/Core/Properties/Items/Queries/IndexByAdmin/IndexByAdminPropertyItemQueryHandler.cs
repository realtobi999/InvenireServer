using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Properties.Items.Queries.IndexByAdmin;

public class IndexByAdminPropertyItemQueryHandler : IRequestHandler<IndexByAdminPropertyItemQuery, IndexByAdminPropertyItemQueryResponse>
{
    private readonly IRepositoryManager _repositories;

    public IndexByAdminPropertyItemQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<IndexByAdminPropertyItemQueryResponse> Handle(IndexByAdminPropertyItemQuery request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        var query = new QueryOptions<PropertyItem, PropertyItemDto>
        {
            Ordering = new QueryOrderingOptions<PropertyItem>(request.Parameters.Order, request.Parameters.Desc),
            Selector = PropertyItemDto.CoreSelector,
            Filtering = new QueryFilteringOptions<PropertyItem>
            {
                Filters =
                [
                    // Core Filter.
                    i => i.PropertyId == property.Id,

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


        return new IndexByAdminPropertyItemQueryResponse
        {
            Data = [.. items],
            Limit = request.Parameters.Limit,
            Offset = request.Parameters.Offset,
            TotalCount = await _repositories.Properties.Items.CountAsync(query.Filtering.Filters!)
        };
    }
}
