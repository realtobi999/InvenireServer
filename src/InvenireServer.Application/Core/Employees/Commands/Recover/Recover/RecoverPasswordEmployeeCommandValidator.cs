using FluentValidation;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Core.Employees.Commands.Recover.Recover;

/// <summary>
/// Defines validation rules for recovering a password for an employee.
/// </summary>
public class RecoverPasswordEmployeeCommandValidator : AbstractValidator<RecoverPasswordEmployeeCommand>
{
    public RecoverPasswordEmployeeCommandValidator()
    {
        RuleFor(c => c.NewPassword)
            .NotEmpty()
            .MinimumLength(Employee.MIN_PASSWORD_LENGTH)
            .MaximumLength(Employee.MAX_PASSWORD_LENGTH)
            .Matches(@"^(?=.*[A-Z])(?=.*\d).+$")
            .WithName("new_password");
        RuleFor(c => c.NewPasswordConfirm)
            .Equal(c => c.NewPassword).WithMessage("'new_password_confirm' must match 'new_password'.")
            .WithName("new_password_confirm");
    }
}
