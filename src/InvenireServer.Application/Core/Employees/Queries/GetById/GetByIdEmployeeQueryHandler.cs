
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Employees.Queries.GetById;

public class GetByIdEmployeeQueryHandler : IRequestHandler<GetByIdEmployeeQuery, EmployeeDto>
{
    private readonly IServiceManager _services;

    public GetByIdEmployeeQueryHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task<EmployeeDto> Handle(GetByIdEmployeeQuery request, CancellationToken ct)
    {
        var admin = await _services.Admins.GetAsync(request.Jwt);
        var employee = await _services.Employees.Dto.GetAsync(e => e.Id == request.EmployeeId);

        if (admin.OrganizationId != employee.OrganizationId) throw new Unauthorized401Exception("The employee is not from the admin's organization.");

        return employee;
    }
}
