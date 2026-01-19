using FluentValidation;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Core.Organizations.Commands.Create;

/// <summary>
/// Defines validation rules for creating an organization.
/// </summary>
public class CreateOrganizationCommandValidator : AbstractValidator<CreateOrganizationCommand>
{
    private readonly IRepositoryManager _repositories;

    public CreateOrganizationCommandValidator(IRepositoryManager repositories)
    {
        _repositories = repositories;

        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(Organization.MAX_NAME_LENGTH)
            .WithName("name")
            .MustAsync(BeUniqueName)
                .WithMessage("'name' must be unique among all organizations.");
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken ct)
    {
        return await _repositories.Organizations.GetAsync(e => e.Name == name) is null;
    }
}