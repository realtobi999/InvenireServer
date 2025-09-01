using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Managers;
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
        => await _repositories.Organizations.GetAndProjectAsync(o => o.Id == request.OrganizationId, OrganizationDto.GetByIdQuerySelector) ?? throw new NotFound404Exception("The organization was not found in the system.");
}
