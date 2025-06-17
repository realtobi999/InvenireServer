using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Domain.Entities;

namespace InvenireServer.Tests.Integration.Extensions;

public static class OrganizationTextExtensions
{
    public static CreateOrganizationDto ToCreateOrganizationDto(this Organization organization)
    {
        var dto = new CreateOrganizationDto
        {
            Id = organization.Id,
            Name = organization.Name,
        };

        return dto;
    }
}