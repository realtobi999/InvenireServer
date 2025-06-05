using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Application.Interfaces.Factories.Admins;
using InvenireServer.Application.Interfaces.Managers;
using Microsoft.AspNetCore.Mvc;

namespace InvenireServer.Presentation.Controllers;

[ApiController]
public class AdminController : ControllerBase
{
    private readonly IAdminFactory _factory;
    private readonly IServiceManager _services;

    public AdminController(IServiceManager services, IFactoryManager factories)
    {
        _factory = factories.Entities.Admins;
        _services = services;
    }

    [HttpPost("/api/auth/admin/register")]
    public async Task<IActionResult> RegisterAdmin(RegisterAdminDto dto)
    {
        var admin = _factory.Create(dto);

        await _services.Admins.CreateAsync(admin);

        return Created($"/api/admin/{admin.Id}", null);
    }
}
