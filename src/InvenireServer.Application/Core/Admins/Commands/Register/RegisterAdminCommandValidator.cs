using FluentValidation;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Core.Admins.Commands.Register;

public class RegisterAdminCommandValidator : AbstractValidator<RegisterAdminCommand>
{
    public RegisterAdminCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(Admin.MAX_NAME_LENGTH)
            .WithName("name");
        RuleFor(c => c.EmailAddress)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(Admin.MAX_EMAIL_ADDRESS_LENGTH)
            .WithName("email_address");
        RuleFor(c => c.Password)
            .NotEmpty()
            .MinimumLength(Admin.MIN_PASSWORD_LENGTH)
            .MaximumLength(Admin.MAX_PASSWORD_LENGTH)
            .Matches(@"^(?=.*[A-Z])(?=.*\d).+$")
            .WithName("password");
        RuleFor(c => c.PasswordConfirm)
            .NotEmpty()
            .Equal(c => c.Password).WithMessage("'password_confirm' must match 'password'.")
            .WithName("password_confirm");
    }
}