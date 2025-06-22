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
    [HttpPost("/api/organizations")]
    public async Task<IActionResult> CreateOrganization(CreateOrganizationDto body)
    {
        // Extract admin from JWT token.
        var jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken());
        var admin = await _services.Admins.GetAsync(jwt);

        // Create the organization and associate it with the admin.
        var organization = _factories.Entities.Organization.Create(body);
        organization.Admin = admin;

        // Save the organization.
        await _services.Organizations.CreateAsync(organization);

        // Update the admin to link with the newly created organization.
        admin.OrganizationId = organization.Id;
        await _services.Admins.UpdateAsync(admin);

        // Send confirmation email to the admin.
        await _services.Admins.SendOrganizationCreationEmail(admin, organization);

        return Created($"/api/organization/{organization.Id}", null);
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/organizations/{organizationId:guid}/invitations")]
    public async Task<IActionResult> CreateOrganizationInvitation(Guid organizationId, CreateOrganizationInvitationDto body)
    {
        // Extract admin from JWT token.
        var jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken());
        var admin = await _services.Admins.GetAsync(jwt);
        var employee = await _services.Employees.GetAsync(e => e.Id == body.EmployeeId);
        var organization = await _services.Organizations.GetAsync(o => o.Id == organizationId);

        // Ensure the admin is the owner of the organization.
        if (admin.Id != organization.Admin!.Id) throw new Unauthorized401Exception();

        // Create and associate the invitation.
        var invitation = _factories.Entities.Organization.Invitation.Create(body);
        invitation.Employee = employee;
        invitation.OrganizationId = organization.Id;

        // Save the invitation.
        await _services.Organizations.Invitations.CreateAsync(invitation);

        // Add invitation to the organization.
        organization.Invitations?.Add(invitation);

        return Created();
    }

    [Authorize(Policy = Jwt.Policies.EMPLOYEE)]
    [HttpPost("/api/organizations/{organizationId:guid}/invitations/{invitationId:guid}/accept")]
    public async Task<IActionResult> AcceptOrganizationInvitation(Guid organizationId, Guid invitationId)
    {
        // Extract employee from JWT token.
        var jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken());
        var employee = await _services.Employees.GetAsync(jwt);
        var invitation = await _services.Organizations.Invitations.GetAsync(i => i.Id == invitationId);
        var organization = await _services.Organizations.GetAsync(o => o.Id == organizationId);

        // Ensure the invitation belongs to the specified organization.
        if (invitation.OrganizationId != organizationId) throw new BadRequest400Exception("The invitation does not belong to the specified organization.");

        // Ensure the invitation is for the authenticated employee.
        if (invitation.Employee!.Id != employee.Id) throw new Unauthorized401Exception();

        // Ensure the employee is not already part of an organization.
        if (employee.OrganizationId is not null) throw new BadRequest400Exception("You are already part of an organization.");

        // Associate the employee with the organization.
        organization.Employees.Add(employee);
        employee.OrganizationId = organization.Id;

        // Delete the invitation
        await _services.Organizations.Invitations.DeleteAsync(invitation);
        organization.Invitations.Remove(invitation);

        // Save the changes.
        await _services.Organizations.UpdateAsync(organization);
        await _services.Employees.UpdateAsync(employee);

        return NoContent();
    }
}
