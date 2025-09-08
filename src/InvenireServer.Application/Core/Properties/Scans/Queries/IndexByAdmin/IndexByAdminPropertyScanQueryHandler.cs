using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;

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

        return new IndexByAdminPropertyScanQueryResponse
        {
            Data = [.. await _repositories.Properties.Scans.IndexAsync(s => s.PropertyId == property.Id, new QueryOptions<PropertyScan, PropertyScanDto>
            {
                Selector = PropertyScanDto.IndexByAdminSelector,
                Pagination = request.Pagination
            })],
            Limit = request.Pagination.Limit,
            Offset = request.Pagination.Offset,
            TotalCount = await _repositories.Properties.Scans.CountAsync(s => s.PropertyId == property.Id)
        };
    }
}
