using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Tests.Integration.Extensions.Organizations;

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
}