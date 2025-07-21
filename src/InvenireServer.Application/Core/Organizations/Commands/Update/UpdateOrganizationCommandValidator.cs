using FluentValidation;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Core.Organizations.Commands.Update;

public class UpdateOrganizationCommandValidator : AbstractValidator<UpdateOrganizationCommand>
{
    private readonly IRepositoryManager _repositories;

    public UpdateOrganizationCommandValidator(IRepositoryManager repositories)
    {
        _repositories = repositories;

        RuleFor(c => c.Name)
            .NotEmpty()
            .MustAsync(BeUniqueName).WithMessage("'name' mut be unique among all organizations.")
            .MaximumLength(Organization.MAX_NAME_LENGTH)
            .WithName("name");
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken _)
    {
        return await _repositories.Organizations.GetAsync(e => e.Name == name) is null;
    }
}
