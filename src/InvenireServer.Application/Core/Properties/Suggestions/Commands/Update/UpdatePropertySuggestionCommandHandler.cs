
using System.Text.Json;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Update;

public class UpdatePropertySuggestionCommandHandler : IRequestHandler<UpdatePropertySuggestionCommand>
{
    private readonly IServiceManager _services;

    public UpdatePropertySuggestionCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task Handle(UpdatePropertySuggestionCommand request, CancellationToken _)
    {
        var suggestion = await _services.Properties.Suggestion.GetAsync(s => s.Id == request.SuggestionId);
        var employee = await _services.Employees.GetAsync(request.Jwt!);
        var organization = await _services.Organizations.TryGetForAsync(employee) ?? throw new BadRequest400Exception("You are not part of an organization.");
        var property = await _services.Properties.TryGetForAsync(organization) ?? throw new BadRequest400Exception("Organization you are part of doesn't have a property.");

        if (suggestion.EmployeeId != employee.Id) throw new Unauthorized401Exception();
        if (suggestion.PropertyId != property.Id) throw new BadRequest400Exception("The suggestion isn't a part of the property.");
        if (suggestion.Status == PropertySuggestionStatus.APPROVED) throw new BadRequest400Exception("The suggestion is approved and cannot be updated.");

        suggestion.Description = request.Description;
        suggestion.PayloadString = JsonSerializer.Serialize(request.Payload);

        if (suggestion.Status == PropertySuggestionStatus.DECLINED)
        {
            suggestion.Feedback = null;
            suggestion.Status = PropertySuggestionStatus.PENDING;
            suggestion.ResolvedAt = null;
        }

        await _services.Properties.Suggestion.UpdateAsync(suggestion);
    }
}
