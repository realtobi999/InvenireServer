using InvenireServer.Domain.Entities;

namespace InvenireServer.Domain.Interfaces.Services.Organizations;

public interface IOrganizationInvitationService
{
    Task CreateAsync(OrganizationInvitation invitation);
}