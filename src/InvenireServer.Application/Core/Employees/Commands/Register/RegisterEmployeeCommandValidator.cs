using FluentValidation;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Core.Employees.Commands.Register;

public class RegisterEmployeeCommandValidator : AbstractValidator<RegisterEmployeeCommand>
{
    public RegisterEmployeeCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(Employee.MAX_NAME_LENGTH)
            .WithName("name");
        RuleFor(c => c.EmailAddress)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(Employee.MAX_EMAIL_ADDRESS_LENGTH)
            .WithName("email_address");
        RuleFor(c => c.Password)
            .NotEmpty()
            .MinimumLength(Employee.MIN_PASSWORD_LENGTH)
            .MaximumLength(Employee.MAX_PASSWORD_LENGTH)
            .Matches(@"^(?=.*[A-Z])(?=.*\d).+$")
            .WithName("password");
        RuleFor(c => c.PasswordConfirm)
            .NotEmpty()
            .Equal(c => c.Password).WithMessage("'password_confirm' must match 'password'.")
            .WithName("password_confirm");
    }
}