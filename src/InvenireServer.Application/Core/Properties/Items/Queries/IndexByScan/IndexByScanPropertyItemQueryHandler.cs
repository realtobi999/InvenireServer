using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Extensions;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

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
        var role = request.Jwt.GetRole();
        var organization = role switch
        {
            Jwt.Roles.ADMIN => await GetOrganizationByAdmin(request.Jwt),
            Jwt.Roles.EMPLOYEE => await GetOrganizationByEmployee(request.Jwt),
            _ => throw new Unauthorized401Exception(),
        };
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        // If the request originates from an employee, ensure no employee filter
        // is applied and then set the filter to the employee’s id.
        if (role == Jwt.Roles.EMPLOYEE)
        {
            if (request.Parameters.EmployeeId is not null) throw new Unauthorized401Exception();
            request.Parameters.EmployeeId = Guid.Parse(request.Jwt.Payload.FirstOrDefault(c => c.Type == "employee_id")?.Value ?? throw new BadRequest400Exception("The JWT is missing 'employee_id' claim."));
        }

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
            Selector = PropertyItemDtoSelector,
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
                    !string.IsNullOrEmpty(request.Parameters.Room) ? i => i.Location.Room == request.Parameters.Room : null,
                    !string.IsNullOrEmpty(request.Parameters.Building) ? i => i.Location.Building == request.Parameters.Building : null,
                    request.Parameters.WithGeneratedCode is not null ? request.Parameters.WithGeneratedCode.Value ? i => i.LastCodeGeneratedAt != null : i => i.LastCodeGeneratedAt == null : null
                ]
            },
            Pagination = new QueryPaginationOptions(request.Parameters.Limit, request.Parameters.Offset)
        };
        var items = await _repositories.Properties.Items.IndexAsync(query);

        // Assign all extra data.
        foreach (var item in items)
        {
            item.LastScannedAt = scan.ScannedItems.First(si => si.PropertyItemId == item.Id).ScannedAt;

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

        return new IndexByScanPropertyItemQueryResponse
        {
            Data = [.. items],
            Limit = request.Parameters.Limit,
            Offset = request.Parameters.Offset,
            TotalCount = await _repositories.Properties.Items.CountAsync(query.Filtering.Filters!)
        };
    }

    private async Task<Organization> GetOrganizationByEmployee(Jwt jwt)
    {
        var employee = await _repositories.Employees.GetAsync(jwt) ?? throw new NotFound404Exception("The employee was not found in the system.");
        return await _repositories.Organizations.GetForAsync(employee) ?? throw new BadRequest400Exception("The employee isn't part of any organization.");
    }

    private async Task<Organization> GetOrganizationByAdmin(Jwt jwt)
    {
        var admin = await _repositories.Admins.GetAsync(jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");
        return await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
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
