using InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Tests.Extensions.Organizations;

/// <summary>
/// Provides test extensions for <see cref="OrganizationInvitation"/>.
/// </summary>
public static class OrganizationInvitationTestExtensions
{
    /// <summary>
    /// Creates a <see cref="CreateOrganizationInvitationCommand"/> from an invitation.
    /// </summary>
    /// <param name="invitation">Source invitation.</param>
    /// <returns>Create organization invitation command.</returns>
    public static CreateOrganizationInvitationCommand ToCreateOrganizationInvitationCommand(this OrganizationInvitation invitation)
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
