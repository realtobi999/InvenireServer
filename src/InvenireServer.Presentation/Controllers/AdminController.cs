using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InvenireServer.Presentation.Controllers;

[ApiController]
public class AdminController : ControllerBase
{
    private readonly IServiceManager _services;
    private readonly IMapper<Admin, RegisterAdminDto> _mapper;

    public AdminController(IServiceManager services, IFactoryManager factories)
    {
        _mapper = factories.Mappers.Initiate<Admin, RegisterAdminDto>();
        _services = services;
    }

    [HttpPost("/api/auth/admin/register")]
    public async Task<IActionResult> RegisterAdmin(RegisterAdminDto dto)
    {
        var admin = _mapper.Map(dto);

        await _services.Admins.CreateAsync(admin);

        return Created($"/api/admin/{admin.Id}", null);
    }
}
