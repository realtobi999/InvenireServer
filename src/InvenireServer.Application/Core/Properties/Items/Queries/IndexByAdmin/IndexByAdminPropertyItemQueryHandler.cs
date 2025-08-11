using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Dtos.Properties;
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

        return new IndexByAdminPropertyItemQueryResponse
        {
            Data = [.. await _repositories.Properties.Items.IndexAndProjectAsync(i => i.PropertyId == property.Id, PropertyItemDto.FromPropertyItemSelector, request.Pagination)],
            Limit = request.Pagination.Limit,
            Offset = request.Pagination.Offset,
            TotalCount = await _repositories.Properties.Items.CountAsync(i => i.PropertyId == property.Id)
        };
    }
}
