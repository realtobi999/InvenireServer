using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Invitations.Queries.GetById;

/// <summary>
/// Handler for the query to get an organization invitation by ID.
/// </summary>
public class GetByIdOrganizationInvitationQueryHandler : IRequestHandler<GetByIdOrganizationInvitationQuery, OrganizationInvitationDto>
{
    private readonly IRepositoryManager _repositories;

    public GetByIdOrganizationInvitationQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the query to get an organization invitation by ID.
    /// </summary>
    /// <param name="request">Query to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task returning the response.</returns>
    public async Task<OrganizationInvitationDto> Handle(GetByIdOrganizationInvitationQuery request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");

        var invitation = await _repositories.Organizations.Invitations.GetAsync(new QueryOptions<OrganizationInvitation, OrganizationInvitationDto>
        {
            Selector = OrganizationInvitationDtoSelector,
            Filtering = new QueryFilteringOptions<OrganizationInvitation>
            {
                Filters =
                [
                    i => i.Id == request.InvitationId
                ]
            }
        }) ?? throw new NotFound404Exception("The invitation was not found in the system.");

        if (invitation.OrganizationId != organization.Id) throw new Unauthorized401Exception("The invitation is not from the admin's organization.");

        return invitation;
    }

    private static Expression<Func<OrganizationInvitation, OrganizationInvitationDto>> OrganizationInvitationDtoSelector
    {
        get
        {
            return i => new OrganizationInvitationDto
            {
                Id = i.Id,
                OrganizationId = i.OrganizationId,
                Description = i.Description,
                CreatedAt = i.CreatedAt,
                LastUpdatedAt = i.LastUpdatedAt,
                Employee = i.Employee == null ? null : new EmployeeDto
                {
                    Id = i.Employee.Id,
                    OrganizationId = i.Employee.OrganizationId,
                    FirstName = i.Employee.FirstName,
                    LastName = i.Employee.LastName,
                    FullName = $"{i.Employee.FirstName} {i.Employee.LastName}",
                    EmailAddress = i.Employee.EmailAddress,
                    CreatedAt = i.Employee.CreatedAt,
                    LastUpdatedAt = i.Employee.LastUpdatedAt,
                }
            };
        }
    }
}
