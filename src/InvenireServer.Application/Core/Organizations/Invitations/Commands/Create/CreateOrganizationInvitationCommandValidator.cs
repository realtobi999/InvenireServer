using FluentValidation;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;

public class CreateOrganizationInvitationCommandValidator : AbstractValidator<CreateOrganizationInvitationCommand>
{
    public CreateOrganizationInvitationCommandValidator()
    {
        RuleFor(c => c.Description)
            .MaximumLength(OrganizationInvitation.MAX_DESCRIPTION_LENGTH)
            .WithName("description");

        RuleFor(c => c.EmployeeEmailAddress)
            .EmailAddress()
            .MaximumLength(Employee.MAX_EMAIL_ADDRESS_LENGTH)
            .WithName("employee_email_address");

        RuleFor(c => c)
            .Must(c => c.EmployeeEmailAddress is not null || c.EmployeeId is not null)
            .WithMessage("Either 'employee_email_address' or 'employee_id' must be provided.");
    }
}