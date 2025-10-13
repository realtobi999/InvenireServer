using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Employees.Queries.GetByEmailAddress;

public class GetByEmailAddressEmployeeQueryHandler : IRequestHandler<GetByEmailAddressEmployeeQuery, EmployeeDto>
{
    private readonly IRepositoryManager _repositories;

    public GetByEmailAddressEmployeeQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<EmployeeDto> Handle(GetByEmailAddressEmployeeQuery request, CancellationToken ct)
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
                    e => e.EmailAddress == request.EmployeeEmailAddress,
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
