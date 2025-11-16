using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Scans.Queries.GetActive.ByAdmin;

public class GetActivePropertyScanQueryHandler : IRequestHandler<GetByAdminActivePropertyScanQuery, PropertyScanDto>
{
    private readonly IRepositoryManager _repositories;

    public GetActivePropertyScanQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<PropertyScanDto> Handle(GetByAdminActivePropertyScanQuery request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        var query = new QueryOptions<PropertyScan, PropertyScanDto>
        {
            Selector = PropertyScanDtoSelector,
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
                    TotalScannedItems = s.ScannedItems.Count(si => si.IsScanned)
                },
            };
        }
    }
}
