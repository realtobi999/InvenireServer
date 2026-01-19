using FluentValidation;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Core.Employees.Commands.Recover.Send;

/// <summary>
/// Defines validation rules for sending a password recovery for an employee.
/// </summary>
public class SendPasswordRecoveryEmployeeCommandValidator : AbstractValidator<SendPasswordRecoveryEmployeeCommand>
{
    public SendPasswordRecoveryEmployeeCommandValidator()
    {
        RuleFor(c => c.EmailAddress)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(Employee.MAX_EMAIL_ADDRESS_LENGTH)
            .WithName("email_address");
    }
}
