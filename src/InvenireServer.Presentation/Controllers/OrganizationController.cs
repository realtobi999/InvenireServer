using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvenireServer.Presentation.Controllers;

[ApiController]
public class OrganizationController : ControllerBase
{
    private readonly IFactoryManager _factories;
    private readonly IServiceManager _services;

    public OrganizationController(IServiceManager services, IFactoryManager factories)
    {
        _services = services;
        _factories = factories;
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/organization")]
    public async Task<IActionResult> CreateOrganization(CreateOrganizationDto body)
    {
        // Get the admin from the JWT.
        var jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken());
        var admin = await _services.Admins.GetAsync(jwt);

        // Initiate the organization entity from the body and assign the admin to it.
        var organization = _factories.Entities.Organization.Create(body);
        organization.Admin = admin;

        // Save to the database.
        await _services.Organizations.CreateAsync(organization);

        // Update the admin assigned organization.
        admin.OrganizationId = organization.Id;
        await _services.Admins.UpdateAsync(admin);

        // Send a confirmation email.
        await _services.Admins.SendOrganizationCreationEmail(admin, organization);

        return Created($"/api/organization/{organization.Id}", null);
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/organization/{organizationId:guid}/invite/{employeeId:guid}")]
    public async Task<IActionResult> CreateOrganizationInvitation(Guid organizationId, Guid employeeId, CreateOrganizationInvitationDto body)
    {
        // Get the admin, employee and the organization.
        var jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken());
        var admin = await _services.Admins.GetAsync(jwt);
        var employee = await _services.Employees.GetAsync(e => e.Id == employeeId);
        var organization = await _services.Organizations.GetAsync(o => o.Id == organizationId);

        // Make sure that the admin is the owner of the organization.
        if (admin.Id != organization.Admin!.Id) throw new Unauthorized401Exception();

        // Initiate the invitation from the body and assign the employee and organization to it.
        var invitation = _factories.Entities.OrganizationInvitation.Create(body);
        invitation.Employee = employee;
        invitation.OrganizationId = organization.Id;

        // Save to the database.
        await _services.OrganizationInvitations.CreateAsync(invitation);

        // Assign the invitation to the organization.
        organization.Invitations?.Add(invitation);

        return Created();
    }
}