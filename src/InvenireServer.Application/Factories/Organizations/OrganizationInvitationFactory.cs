using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Factories.Organizations;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Factories.Organizations;

public class OrganizationInvitationFactory : IOrganizationInvitationFactory
{
    public OrganizationInvitation Create(CreateOrganizationInvitationDto dto)
    {
        var invitation = new OrganizationInvitation
        {
            Id = dto.Id ?? Guid.NewGuid(),
            Description = dto.Description,
            CreatedAt = DateTimeOffset.UtcNow,
            LastUpdatedAt = null
        };

        return invitation;
    }
}