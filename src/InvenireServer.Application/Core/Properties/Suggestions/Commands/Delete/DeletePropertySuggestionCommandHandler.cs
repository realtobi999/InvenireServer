
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Delete;

public class DeletePropertySuggestionCommandHandler : IRequestHandler<DeletePropertySuggestionCommand>
{
    private readonly IServiceManager _services;

    public DeletePropertySuggestionCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(DeletePropertySuggestionCommand request, CancellationToken _)
    {
        var suggestion = await _services.Properties.Suggestion.GetAsync(s => s.Id == request.SuggestionId);

        switch (request.Jwt.GetRole())
        {
            case Jwt.Roles.ADMIN:
                await ValidateForAdminAsync(request.Jwt, suggestion);
                break;
            case Jwt.Roles.EMPLOYEE:
                await ValidateForEmployeeAsync(request.Jwt, suggestion);
                break;
        }

        await _services.Properties.Suggestion.DeleteAsync(suggestion);
    }

    private async Task ValidateForAdminAsync(Jwt jwt, PropertySuggestion suggestion)
    {
        var admin = await _services.Admins.GetAsync(jwt);
        var organization = await _services.Organizations.TryGetForAsync(admin) ?? throw new BadRequest400Exception("You do not own a organization.");
        var property = await _services.Properties.TryGetForAsync(organization) ?? throw new BadRequest400Exception("You have not created a property.");

        if (suggestion.PropertyId != property.Id) throw new BadRequest400Exception("The suggestion isn't a part your property.");
    }

    private async Task ValidateForEmployeeAsync(Jwt jwt, PropertySuggestion suggestion)
    {
        var employee = await _services.Employees.GetAsync(jwt);
        var organization = await _services.Organizations.TryGetForAsync(employee) ?? throw new BadRequest400Exception("You are not part of an organization.");
        var property = await _services.Properties.TryGetForAsync(organization) ?? throw new BadRequest400Exception("Organization you are part of doesn't have a property.");

        if (suggestion.Status == PropertySuggestionStatus.APPROVED) throw new Unauthorized401Exception();
        if (suggestion.EmployeeId != employee.Id) throw new Unauthorized401Exception();
        if (suggestion.PropertyId != property.Id) throw new BadRequest400Exception("The suggestion isn't a part of the property.");
    }
}
