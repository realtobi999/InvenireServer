using FluentValidation;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Core.Admins.Commands.Recover.Recover;

/// <summary>
/// Defines validation rules for recovering a password for an admin.
/// </summary>
public class RecoverPasswordAdminCommandValidator : AbstractValidator<RecoverPasswordAdminCommand>
{
    public RecoverPasswordAdminCommandValidator()
    {
        RuleFor(c => c.NewPassword)
            .NotEmpty()
            .MinimumLength(Admin.MIN_PASSWORD_LENGTH)
            .MaximumLength(Admin.MAX_PASSWORD_LENGTH)
            .Matches(@"^(?=.*[A-Z])(?=.*\d).+$")
            .WithName("new_password");
        RuleFor(c => c.NewPasswordConfirm)
            .Equal(c => c.NewPassword).WithMessage("'new_password_confirm' must match 'new_password'.")
            .WithName("new_password_confirm");
    }
}
