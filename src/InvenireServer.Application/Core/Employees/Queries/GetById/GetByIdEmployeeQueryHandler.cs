using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Employees.Queries.GetById;

/// <summary>
/// Handler for the query to get an employee by ID.
/// </summary>
public class GetByIdEmployeeQueryHandler : IRequestHandler<GetByIdEmployeeQuery, EmployeeDto>
{
    private readonly IRepositoryManager _repositories;

    public GetByIdEmployeeQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the query to get an employee by ID.
    /// </summary>
    /// <param name="request">Query to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task returning the response.</returns>
    public async Task<EmployeeDto> Handle(GetByIdEmployeeQuery request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");

        var employee = await _repositories.Employees.GetAsync(new QueryOptions<Employee, EmployeeDto>
        {
            Selector = EmployeeDtoSelector,
            Filtering = new QueryFilteringOptions<Employee>
            {
                Filters =
                [
                    e => e.Id == request.EmployeeId,
                ]
            }
        }) ?? throw new NotFound404Exception("The employee was not found in the system.");

        if (employee.OrganizationId != organization.Id) throw new Unauthorized401Exception("The employee is not from the admin's organization.");

        return employee;
    }

    private static Expression<Func<Employee, EmployeeDto>> EmployeeDtoSelector
    {
        get
        {
            return e => new EmployeeDto
            {
                Id = e.Id,
                OrganizationId = e.OrganizationId,
                FirstName = e.FirstName,
                LastName = e.LastName,
                FullName = $"{e.FirstName} {e.LastName}",
                EmailAddress = e.EmailAddress,
                CreatedAt = e.CreatedAt,
                LastUpdatedAt = e.LastUpdatedAt,
            };
        }
    }
}
