using FluentValidation;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Core.Admins.Commands.Update;

public class UpdateAdminCommandValidator : AbstractValidator<UpdateAdminCommand>
{
    public UpdateAdminCommandValidator()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty()
            .MinimumLength(Admin.MIN_NAME_LENGTH)
            .MaximumLength(Admin.MAX_NAME_LENGTH)
            .Matches(@"^\p{L}+$")
            .WithName("first_name");
        RuleFor(c => c.LastName)
            .NotEmpty()
            .MinimumLength(Admin.MIN_NAME_LENGTH)
            .MaximumLength(Admin.MAX_NAME_LENGTH)
            .Matches(@"^\p{L}+$")
            .WithName("last_name");
    }
}
