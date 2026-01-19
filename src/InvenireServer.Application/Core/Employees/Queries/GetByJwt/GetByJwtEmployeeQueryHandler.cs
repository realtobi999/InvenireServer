using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Employees.Queries.GetByJwt;

/// <summary>
/// Handler for the query to get an employee using a JWT.
/// </summary>
public class GetByJwtEmployeeQueryHandler : IRequestHandler<GetByJwtEmployeeQuery, EmployeeDto>
{
    private readonly IRepositoryManager _repositories;

    public GetByJwtEmployeeQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the query to get an employee using a JWT.
    /// </summary>
    /// <param name="request">Query to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task returning the response.</returns>
    public async Task<EmployeeDto> Handle(GetByJwtEmployeeQuery request, CancellationToken ct)
    {
        var employee = await _repositories.Employees.GetAsync(request.Jwt, new QueryOptions<Employee, EmployeeDto>
        {
            Selector = EmployeeDtoSelector
        }) ?? throw new NotFound404Exception("The employee was not found in the system.");

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
