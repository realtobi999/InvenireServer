using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Items.Queries.IndexByAdmin;

/// <summary>
/// Handler for the query to index property items for an admin.
/// </summary>
public class IndexByAdminPropertyItemQueryHandler : IRequestHandler<IndexByAdminPropertyItemQuery, IndexByAdminPropertyItemQueryResponse>
{
    private readonly IRepositoryManager _repositories;

    public IndexByAdminPropertyItemQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the query to index property items for an admin.
    /// </summary>
    /// <param name="request">Query to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task returning the response.</returns>
    public async Task<IndexByAdminPropertyItemQueryResponse> Handle(IndexByAdminPropertyItemQuery request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        var query = new QueryOptions<PropertyItem, PropertyItemDto>
        {
            Selector = PropertyItemDtoSelector,
            Ordering = new QueryOrderingOptions<PropertyItem>(request.Parameters.Order, request.Parameters.Desc),
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
                    !string.IsNullOrEmpty(request.Parameters.Room) ? i => i.Location.Room == request.Parameters.Room : null,
                    !string.IsNullOrEmpty(request.Parameters.Building) ? i => i.Location.Building == request.Parameters.Building : null,
                    request.Parameters.WithGeneratedCode is not null ? request.Parameters.WithGeneratedCode.Value ? i => i.LastCodeGeneratedAt != null : i => i.LastCodeGeneratedAt == null : null
                ]
            },
            Pagination = new QueryPaginationOptions(request.Parameters.Limit, request.Parameters.Offset)
        };

        var items = await _repositories.Properties.Items.IndexAsync(query);

        // Load all the employees.
        foreach (var item in items)
        {
            if (item.EmployeeId is null) continue;
            item.Employee = await _repositories.Employees.GetAsync(new QueryOptions<Employee, EmployeeDto>
            {
                Selector = EmployeeDtoSelector,
                Filtering = new QueryFilteringOptions<Employee>
                {
                    Filters = [e => e.Id == item.EmployeeId]
                },
            }) ?? throw new NotFound404Exception("The employee was not found in the system.");
        }

        return new IndexByAdminPropertyItemQueryResponse
        {
            Data = [.. items],
            Limit = request.Parameters.Limit,
            Offset = request.Parameters.Offset,
            TotalCount = await _repositories.Properties.Items.CountAsync(query.Filtering.Filters!)
        };
    }

    private static Expression<Func<PropertyItem, PropertyItemDto>> PropertyItemDtoSelector
    {
        get
        {
            return i => new PropertyItemDto
            {
                Id = i.Id,
                PropertyId = i.PropertyId,
                EmployeeId = i.EmployeeId,
                InventoryNumber = i.InventoryNumber,
                RegistrationNumber = i.RegistrationNumber,
                Name = i.Name,
                Price = i.Price,
                SerialNumber = i.SerialNumber,
                DateOfPurchase = i.DateOfPurchase,
                DateOfSale = i.DateOfSale,
                Location = new PropertyItemLocationDto
                {
                    Room = i.Location.Room,
                    Building = i.Location.Building,
                    AdditionalNote = i.Location.AdditionalNote,
                },
                Description = i.Description,
                DocumentNumber = i.DocumentNumber,
                CreatedAt = i.CreatedAt,
                LastUpdatedAt = i.LastUpdatedAt,
                LastCodeGeneratedAt = i.LastCodeGeneratedAt,
            };
        }
    }

    private static Expression<Func<Employee, EmployeeDto>> EmployeeDtoSelector
    {
        get
        {
            return e => new EmployeeDto
            {
                Id = e.Id,
                OrganizationId = e.OrganizationId,
                FirstName = e.FirstName,
                LastName = e.LastName,
                FullName = $"{e.FirstName} {e.LastName}",
                EmailAddress = e.EmailAddress,
                CreatedAt = e.CreatedAt,
                LastUpdatedAt = e.LastUpdatedAt,
            };
        }
    }
}
