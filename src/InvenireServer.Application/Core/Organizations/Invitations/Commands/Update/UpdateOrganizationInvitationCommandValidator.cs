using FluentValidation;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Update;

public class UpdateOrganizationInvitationCommandValidator : AbstractValidator<UpdateOrganizationInvitationCommand>
{
    public UpdateOrganizationInvitationCommandValidator()
    {
        RuleFor(c => c.Description)
            .MaximumLength(OrganizationInvitation.MAX_DESCRIPTION_LENGTH)
            .WithName("description");
    }
}
