using FluentValidation;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Core.Employees.Commands.Update;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty()
            .MinimumLength(Employee.MIN_NAME_LENGTH)
            .MaximumLength(Employee.MAX_NAME_LENGTH)
            .Matches(@"^\p{L}+$")
            .WithName("first_name");
        RuleFor(c => c.LastName)
            .NotEmpty()
            .MinimumLength(Employee.MIN_NAME_LENGTH)
            .MaximumLength(Employee.MAX_NAME_LENGTH)
            .Matches(@"^\p{L}+$")
            .WithName("last_name");
    }
}
