using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Queries.GetByAdmin;

/// <summary>
/// Handler for the query to get an organization for an admin.
/// </summary>
public class GetByAdminOrganizationQueryHandler : IRequestHandler<GetByAdminOrganizationQuery, OrganizationDto>
{
    private readonly IRepositoryManager _repositories;

    public GetByAdminOrganizationQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the query to get an organization for an admin.
    /// </summary>
    /// <param name="request">Query to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task returning the response.</returns>
    public async Task<OrganizationDto> Handle(GetByAdminOrganizationQuery request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system");

        return await _repositories.Organizations.GetAsync(new QueryOptions<Organization, OrganizationDto>()
        {
            Selector = OrganizationDtoSelector,
            Filtering = new QueryFilteringOptions<Organization>
            {
                Filters =
                [
                    o => o.Id == admin.OrganizationId
                ]
            }
        }) ?? throw new NotFound404Exception("The admin doesn't own a organization");
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
                    FirstName = o.Admin.FirstName,
                    LastName = o.Admin.LastName,
                    FullName = $"{o.Admin.FirstName} {o.Admin.LastName}",
                    EmailAddress = o.Admin.EmailAddress,
                    CreatedAt = o.Admin.CreatedAt,
                    LastUpdatedAt = o.Admin.LastUpdatedAt
                },
                Property = o.Property == null ? null : new PropertyDto
                {
                    Id = o.Property.Id,
                    CreatedAt = o.Property.CreatedAt,
                    LastUpdatedAt = o.LastUpdatedAt,
                },
                Employees = o.Employees.Count == 0 ? null : o.Employees.Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    FullName = $"{e.FirstName} {e.LastName}",
                    EmailAddress = e.EmailAddress,
                    CreatedAt = e.CreatedAt,
                    LastUpdatedAt = e.LastUpdatedAt,
                }).ToList(),
                Invitations = o.Invitations.Count == 0 ? null : o.Invitations.Select(i => new OrganizationInvitationDto
                {

                    Id = i.Id,
                    Description = i.Description,
                    CreatedAt = i.CreatedAt,
                    LastUpdatedAt = i.LastUpdatedAt,
                    Employee = i.Employee == null ? null : new EmployeeDto
                    {
                        Id = i.Employee.Id,
                        FirstName = i.Employee.FirstName,
                        LastName = i.Employee.LastName,
                        FullName = $"{i.Employee.FirstName} {i.Employee.LastName}",
                        EmailAddress = i.Employee.EmailAddress,
                        CreatedAt = i.Employee.CreatedAt,
                        LastUpdatedAt = i.Employee.LastUpdatedAt,
                    },
                }).ToList()
            };
        }
    }
}