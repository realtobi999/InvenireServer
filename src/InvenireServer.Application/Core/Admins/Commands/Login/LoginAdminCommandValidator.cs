using FluentValidation;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Core.Admins.Commands.Login;

/// <summary>
/// Defines validation rules for authenticating an admin.
/// </summary>
public class LoginAdminCommandValidator : AbstractValidator<LoginAdminCommand>
{
    public LoginAdminCommandValidator()
    {
        RuleFor(c => c.EmailAddress)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(Admin.MAX_EMAIL_ADDRESS_LENGTH)
            .WithName("email_address");
        RuleFor(c => c.Password)
            .NotEmpty()
            .MinimumLength(Admin.MIN_PASSWORD_LENGTH)
            .MaximumLength(Admin.MAX_PASSWORD_LENGTH)
            .WithName("password");
    }
}