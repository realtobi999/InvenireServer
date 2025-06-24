using System;
using InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Tests.Integration.Extensions.Organizations;

public static class OrganizationInvitationTestExtensions
{
    public static CreateOrganizationInvitationCommand ToCreateOrganizationInvitationDto(this OrganizationInvitation invitation)
    {
        var dto = new CreateOrganizationInvitationCommand
        {
            Id = invitation.Id,
            Description = invitation.Description,
            EmployeeId = (invitation.Employee ?? throw new NullReferenceException("Employee not set.")).Id
        };

        return dto;
    }
}
