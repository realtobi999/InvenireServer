
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Employees.Queries.GetById;

public class GetByIdEmployeeQueryHandler : IRequestHandler<GetByIdEmployeeQuery, EmployeeDto>
{
    private readonly IRepositoryManager _repositories;

    public GetByIdEmployeeQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<EmployeeDto> Handle(GetByIdEmployeeQuery request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");

        var employee = await _repositories.Employees.GetAsync(e => e.Id == request.EmployeeId, new QueryOptions<Employee, EmployeeDto>
        {
            Selector = EmployeeDto.FromEmployeeSelector
        }) ?? throw new NotFound404Exception("The employee was not found in the system.");

        if (employee.OrganizationId != organization.Id) throw new Unauthorized401Exception("The employee is not from the admin's organization.");

        return employee;
    }
}
