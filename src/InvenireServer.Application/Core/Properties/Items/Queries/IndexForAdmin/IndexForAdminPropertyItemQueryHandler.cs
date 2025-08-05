using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Items.Queries.IndexForAdmin;

public class IndexForAdminPropertyItemQueryHandler : IRequestHandler<IndexForAdminPropertyItemQuery, IEnumerable<PropertyItemDto>>
{
    private readonly IServiceManager _services;

    public IndexForAdminPropertyItemQueryHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task<IEnumerable<PropertyItemDto>> Handle(IndexForAdminPropertyItemQuery request, CancellationToken ct)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt!);
        var organization = await _services.Organizations.TryGetForAsync(admin) ?? throw new BadRequest400Exception("You do not own a organization.");
        var property = await _services.Properties.TryGetForAsync(organization) ?? throw new BadRequest400Exception("You have not created a property.");

        var items = await _services.Properties.Items.Dto.IndexForAsync(property, request.Pagination);

        return items;
    }
}
