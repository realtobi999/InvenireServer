using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Queries.GetById;

/// <summary>
/// Handler for the query to get an organization by ID.
/// </summary>
public class GetByIdOrganizationQueryHandler : IRequestHandler<GetByIdOrganizationQuery, OrganizationDto>
{
    private readonly IRepositoryManager _repositories;

    public GetByIdOrganizationQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the query to get an organization by ID.
    /// </summary>
    /// <param name="request">Query to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task returning the response.</returns>
    public async Task<OrganizationDto> Handle(GetByIdOrganizationQuery request, CancellationToken ct)
    {
        return await _repositories.Organizations.GetAsync(new QueryOptions<Organization, OrganizationDto>
        {
            Selector = OrganizationDtoSelector,
            Filtering = new QueryFilteringOptions<Organization>
            {
                Filters =
                [
                    o => o.Id == request.OrganizationId
                ]
            }
        }) ?? throw new NotFound404Exception("The organization was not found in the system.");
    }

    private static Expression<Func<Organization, OrganizationDto>> OrganizationDtoSelector
    {
        get
        {
            return o => new OrganizationDto
            {
                Id = o.Id,
                Name = o.Name,
                CreatedAt = o.CreatedAt,
                LastUpdatedAt = o.LastUpdatedAt,
                Admin = o.Admin == null ? null : new AdminDto
                {
                    Id = o.Admin.Id,
                    OrganizationId = o.Admin.OrganizationId,
                    FirstName = o.Admin.FirstName,
                    LastName = o.Admin.LastName,
                    FullName = $"{o.Admin.FirstName} {o.Admin.LastName}",
                    EmailAddress = o.Admin.EmailAddress,
                    CreatedAt = o.Admin.CreatedAt,
                    LastUpdatedAt = o.Admin.LastUpdatedAt
                },
            };
        }
    }
}
