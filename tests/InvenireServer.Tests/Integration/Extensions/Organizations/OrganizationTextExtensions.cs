using InvenireServer.Application.Core.Organizations.Commands.Create;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Tests.Integration.Extensions.Organizations;

public static class OrganizationTextExtensions
{
    public static CreateOrganizationCommand ToCreateOrganizationCommand(this Organization organization)
    {
        var dto = new CreateOrganizationCommand
        {
            Id = organization.Id,
            Name = organization.Name,
            Jwt = null,
            FrontendBaseUrl = null
        };

        return dto;
    }
}