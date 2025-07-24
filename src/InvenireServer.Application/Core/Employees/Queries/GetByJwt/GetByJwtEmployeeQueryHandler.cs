
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Employees.Queries.GetByJwt;

public class GetByJwtEmployeeQueryHandler : IRequestHandler<GetByJwtEmployeeQuery, EmployeeDto>
{
    private readonly IServiceManager _services;

    public GetByJwtEmployeeQueryHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task<EmployeeDto> Handle(GetByJwtEmployeeQuery request, CancellationToken ct) => await _services.Employees.Dto.GetAsync(request.Jwt);
}
