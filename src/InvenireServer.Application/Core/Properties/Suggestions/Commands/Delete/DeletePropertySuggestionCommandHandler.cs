
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Delete;

public class DeletePropertySuggestionCommandHandler : IRequestHandler<DeletePropertySuggestionCommand>
{
    private readonly IRepositoryManager _repositories;

    public DeletePropertySuggestionCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task Handle(DeletePropertySuggestionCommand request, CancellationToken ct)
    {
        var suggestion = await _repositories.Properties.Suggestions.GetAsync(s => s.Id == request.SuggestionId) ?? throw new NotFound404Exception("The suggestion was not found in the system.");

        switch (request.Jwt.GetRole())
        {
            case Jwt.Roles.ADMIN:
                await ValidateForAdminAsync(request.Jwt, suggestion);
                break;
            case Jwt.Roles.EMPLOYEE:
                await ValidateForEmployeeAsync(request.Jwt, suggestion);
                break;
        }

        _repositories.Properties.Suggestions.Delete(suggestion);

        await _repositories.SaveOrThrowAsync();
    }

    private async Task ValidateForAdminAsync(Jwt jwt, PropertySuggestion suggestion)
    {
        var admin = await _repositories.Admins.GetAsync(jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        if (suggestion.PropertyId != property.Id) throw new BadRequest400Exception("The suggestion isn't a part the property.");
    }

    private async Task ValidateForEmployeeAsync(Jwt jwt, PropertySuggestion suggestion)
    {
        var employee = await _repositories.Employees.GetAsync(jwt) ?? throw new NotFound404Exception("The employee was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(employee) ?? throw new BadRequest400Exception("The employee isn't part of any organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        if (suggestion.Status == PropertySuggestionStatus.APPROVED) throw new Unauthorized401Exception("The suggestion was already approved by the admin.");
        if (suggestion.EmployeeId != employee.Id) throw new Unauthorized401Exception("The suggestion doesn't belong to the employee.");
        if (suggestion.PropertyId != property.Id) throw new BadRequest400Exception("The suggestion isn't a part of the property.");
    }
}
