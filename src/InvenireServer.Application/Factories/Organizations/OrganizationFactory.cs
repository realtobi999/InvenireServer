using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Factories.Organizations;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Factories.Organizations;

public class OrganizationFactory : IOrganizationFactory
{
    public IOrganizationInvitationFactory Invitation => new OrganizationInvitationFactory();

    public Organization Create(CreateOrganizationDto dto)
    {
        var organization = new Organization
        {
            Id = dto.Id ?? Guid.NewGuid(),
            Name = dto.Name,
            CreatedAt = DateTimeOffset.UtcNow,
            LastUpdatedAt = null
        };

        return organization;
    }
}