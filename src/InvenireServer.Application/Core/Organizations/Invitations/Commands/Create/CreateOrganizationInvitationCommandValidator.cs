using FluentValidation;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;

public class CreateOrganizationInvitationCommandValidator : AbstractValidator<CreateOrganizationInvitationCommand>
{
    public CreateOrganizationInvitationCommandValidator()
    {
        RuleFor(c => c.Description)
            .MaximumLength(OrganizationInvitation.MAX_DESCRIPTION_LENGTH)
            .WithName("description");
        RuleFor(c => c.EmployeeId)
            .NotEmpty()
            .WithName("employee_id");
    }
}