
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Employees.Queries.GetByJwt;

public class GetByJwtEmployeeQueryHandler : IRequestHandler<GetByJwtEmployeeQuery, EmployeeDto>
{
    private readonly IRepositoryManager _repositories;

    public GetByJwtEmployeeQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<EmployeeDto> Handle(GetByJwtEmployeeQuery request, CancellationToken ct)
        => await _repositories.Employees.GetAsync(request.Jwt, new QueryOptions<Employee, EmployeeDto>
        {
            Selector = EmployeeDto.FromEmployeeSelector
        }) ?? throw new NotFound404Exception("The employee was not found in the system.");
}
