using FluentValidation;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Organizations.Commands.Update;

public class UpdateOrganizationCommandValidator : AbstractValidator<UpdateOrganizationCommand>
{
    private readonly IRepositoryManager _repositories;

    public UpdateOrganizationCommandValidator(IRepositoryManager repositories)
    {
        _repositories = repositories;

        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(Organization.MAX_NAME_LENGTH)
            .WithName("name")
            .MustAsync(BeUniqueName)
                .WithMessage("'name' must be unique among all organizations.");
    }


    private async Task<bool> BeUniqueName(UpdateOrganizationCommand request, string name, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The admin was not found in the system.");
        return await _repositories.Organizations.GetAsync(o => o.Name == name && o.Id != admin.OrganizationId) is null;
    }
}
