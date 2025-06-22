using System;
using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Tests.Integration.Extensions.Organizations;

public static class OrganizationInvitationTestExtensions
{
    public static CreateOrganizationInvitationDto ToCreateOrganizationInvitationDto(this OrganizationInvitation invitation)
    {
        var dto = new CreateOrganizationInvitationDto
        {
            Id = invitation.Id,
            Description = invitation.Description,
            EmployeeId = (invitation.Employee ?? throw new NullReferenceException("Employee not set.")).Id
        };

        return dto;
    }
}
