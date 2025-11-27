using FluentValidation;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Core.Admins.Commands.Recover.Send;

public class SendPasswordRecoveryAdminCommandValidator : AbstractValidator<SendPasswordRecoveryAdminCommand>
{
    public SendPasswordRecoveryAdminCommandValidator()
    {
        RuleFor(c => c.EmailAddress)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(Admin.MAX_EMAIL_ADDRESS_LENGTH)
            .WithName("email_address");
    }
}
