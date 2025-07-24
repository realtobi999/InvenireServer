
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Employees.Queries.GetById;

public class GetByIdEmployeeQueryHandler : IRequestHandler<GetByIdEmployeeQuery, EmployeeDto>
{
    private readonly IServiceManager _services;

    public GetByIdEmployeeQueryHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task<EmployeeDto> Handle(GetByIdEmployeeQuery request, CancellationToken ct) => await _services.Employees.Dto.GetAsync(e => e.Id == request.EmployeeId);
}
