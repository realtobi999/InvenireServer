using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Tests.Integration.Extensions;

public static class OrganizationTextExtensions
{
    public static CreateOrganizationDto ToCreateOrganizationDto(this Organization organization)
    {
        var dto = new CreateOrganizationDto
        {
            Id = organization.Id,
            Name = organization.Name
        };

        return dto;
    }

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