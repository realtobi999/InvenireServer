using FluentValidation;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Core.Employees.Commands.Register;

public class RegisterEmployeeCommandValidator : AbstractValidator<RegisterEmployeeCommand>
{
    private readonly IRepositoryManager _repositories;

    public RegisterEmployeeCommandValidator(IRepositoryManager repositories)
    {
        _repositories = repositories;

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
        RuleFor(c => c.EmailAddress)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(BeUniqueEmail).WithMessage("'email_address' must be unique among all users.")
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

    private async Task<bool> BeUniqueEmail(string email, CancellationToken ct)
    {
        return await _repositories.Employees.GetAsync(e => e.EmailAddress == email) is null;
    }
}