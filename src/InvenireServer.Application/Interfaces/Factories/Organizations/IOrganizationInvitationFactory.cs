using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Interfaces.Factories.Organizations;

public interface IOrganizationInvitationFactory
{
    OrganizationInvitation Create(CreateOrganizationInvitationDto dto);
}