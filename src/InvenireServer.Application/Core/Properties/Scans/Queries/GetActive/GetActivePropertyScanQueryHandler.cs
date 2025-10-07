using System.Linq.Expressions;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Properties.Scans.Queries.GetActive;

public class GetActivePropertyScanQueryHandler : IRequestHandler<GetActivePropertyScanQuery, PropertyScanDto>
{
    private readonly IRepositoryManager _repositories;

    public GetActivePropertyScanQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<PropertyScanDto> Handle(GetActivePropertyScanQuery request, CancellationToken ct)
    {
        var organization = request.Jwt.GetRole() switch
        {
            Jwt.Roles.ADMIN => await GetOrganizationByAdmin(request.Jwt),
            Jwt.Roles.EMPLOYEE => await GetOrganizationByEmployee(request.Jwt),
            _ => throw new Unauthorized401Exception(),
        };
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        var query = new QueryOptions<PropertyScan, PropertyScanDto>
        {
            Selector = Selector,
            Filtering = new QueryFilteringOptions<PropertyScan>
            {
                Filters =
                [
                    s => s.PropertyId == property.Id && s.Status == PropertyScanStatus.IN_PROGRESS,
                ]
            },
        };
        var scan = await _repositories.Properties.Scans.GetAsync(query) ?? throw new NotFound404Exception("The property doesn't have a active scan.");

        return scan;
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

    private static Expression<Func<PropertyScan, PropertyScanDto>> Selector
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
