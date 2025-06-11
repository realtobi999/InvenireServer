using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Factories.Organizations;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvenireServer.Presentation.Controllers;

[ApiController]
public class OrganizationController : ControllerBase
{
    private readonly IServiceManager _services;
    private readonly IOrganizationFactory _factory;

    public OrganizationController(IServiceManager services, IFactoryManager factories)
    {
        _services = services;
        _factory = factories.Entities.Organization;
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/organization")]
    public async Task<IActionResult> CreateOrganization(CreateOrganizationDto body)
    {
        // Get the admin from the JWT.
        var jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken());
        var admin = await _services.Admins.GetAsync(jwt);

        // Initiate the organization entity from the body and assign the admin to it.
        var organization = _factory.Create(body);
        organization.Admin = admin;

        // Save to the database.
        await _services.Organizations.CreateAsync(organization);

        // Update the admin assigned organization.
        admin.OrganizationId = organization.Id;
        await _services.Admins.UpdateAsync(admin);

        return Created($"/api/organization/{organization.Id}", null);
    }
}
