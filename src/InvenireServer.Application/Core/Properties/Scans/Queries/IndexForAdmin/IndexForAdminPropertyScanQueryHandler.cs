using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Properties.Scans.Queries.IndexForAdmin;

public class IndexForAdminPropertyScanQueryHandler : IRequestHandler<IndexForAdminPropertyScanQuery, IndexForAdminPropertyScanQueryResponse>
{
    private readonly IRepositoryManager _repositories;

    public IndexForAdminPropertyScanQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<IndexForAdminPropertyScanQueryResponse> Handle(IndexForAdminPropertyScanQuery request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new NotFound404Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new NotFound404Exception("The organization owned by the admin doesn't have a property created.");

        return new IndexForAdminPropertyScanQueryResponse
        {
            Data = [.. await _repositories.Properties.Scans.IndexAndProjectToAsync(s => s.PropertyId == property.Id, PropertyScanDto.IndexForAdminSelector, request.Pagination)],
            Limit = request.Pagination.Limit,
            Offset = request.Pagination.Offset,
            TotalCount = await _repositories.Properties.Scans.CountAsync(s => s.PropertyId == property.Id)
        };
    }
}
