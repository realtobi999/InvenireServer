using FluentValidation;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Core.Employees.Commands.Login;

/// <summary>
/// Defines validation rules for authenticating an employee.
/// </summary>
public class LoginEmployeeCommandValidator : AbstractValidator<LoginEmployeeCommand>
{
    public LoginEmployeeCommandValidator()
    {
        RuleFor(c => c.EmailAddress)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(Employee.MAX_EMAIL_ADDRESS_LENGTH)
            .WithName("email_address");
        RuleFor(c => c.Password)
            .NotEmpty()
            .MaximumLength(Employee.MAX_PASSWORD_LENGTH)
            .WithName("password");
    }
}