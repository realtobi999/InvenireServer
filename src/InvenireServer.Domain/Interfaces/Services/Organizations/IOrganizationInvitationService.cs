using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Domain.Interfaces.Services.Organizations;

public interface IOrganizationInvitationService
{
    Task CreateAsync(OrganizationInvitation invitation);
}