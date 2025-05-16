using Microsoft.AspNetCore.Mvc;
using InvenireServer.Domain.Core.Entities;
using InvenireServer.Domain.Core.Dtos.Employees;
using InvenireServer.Domain.Core.Interfaces.Common;
using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Domain.Core.Interfaces.Factories;

namespace InvenireServer.Presentation.Controllers;

[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IServiceManager _services;
    private readonly IMapper<Employee, RegisterEmployeeDto> _mapper;

    public EmployeeController(IServiceManager services, IMapperFactory factory)
    {
        _mapper = factory.Initiate<Employee, RegisterEmployeeDto>();
        _services = services;
    }

    [HttpPost("/api/auth/employee/register")]
    public async Task<IActionResult> RegisterEmployee(RegisterEmployeeDto dto)
    {
        var employee = _mapper.Map(dto);
        await _services.Employee.CreateAsync(employee);
        return Created($"/api/employee/{employee.Id}", null);
    }
}
