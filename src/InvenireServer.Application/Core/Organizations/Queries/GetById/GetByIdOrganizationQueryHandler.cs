using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Queries.GetById;

public class GetByIdOrganizationQueryHandler : IRequestHandler<GetByIdOrganizationQuery, OrganizationDto>
{
    private readonly IRepositoryManager _repositories;

    public GetByIdOrganizationQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<OrganizationDto> Handle(GetByIdOrganizationQuery request, CancellationToken ct)
        => await _repositories.Organizations.GetAsync(o => o.Id == request.OrganizationId, new QueryOptions<Organization, OrganizationDto>
        {
            Selector = OrganizationDto.GetByIdQuerySelector
        }) ?? throw new NotFound404Exception("The organization was not found in the system.");
}
