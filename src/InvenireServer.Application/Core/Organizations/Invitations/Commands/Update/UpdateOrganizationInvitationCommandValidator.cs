using FluentValidation;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Update;

/// <summary>
/// Defines validation rules for updating an organization invitation.
/// </summary>
public class UpdateOrganizationInvitationCommandValidator : AbstractValidator<UpdateOrganizationInvitationCommand>
{
    public UpdateOrganizationInvitationCommandValidator()
    {
        RuleFor(c => c.Description)
            .MaximumLength(OrganizationInvitation.MAX_DESCRIPTION_LENGTH)
            .WithName("description");
    }
}
