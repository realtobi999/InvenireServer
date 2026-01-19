using System.Text.Json;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Update;

/// <summary>
/// Handler for the request to update a property suggestion.
/// </summary>
public class UpdatePropertySuggestionCommandHandler : IRequestHandler<UpdatePropertySuggestionCommand>
{
    private readonly IRepositoryManager _repositories;

    public UpdatePropertySuggestionCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the request to update a property suggestion.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task representing the operation.</returns>
    public async Task Handle(UpdatePropertySuggestionCommand request, CancellationToken ct)
    {
        var suggestion = await _repositories.Properties.Suggestions.GetAsync(s => s.Id == request.SuggestionId) ?? throw new NotFound404Exception("The suggestion was not found in the system.");
        var employee = await _repositories.Employees.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The employee was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(employee) ?? throw new BadRequest400Exception("The employee isn't part of any organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        if (suggestion.EmployeeId != employee.Id) throw new Unauthorized401Exception("The suggestion doesn't belong to the employee.");
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

        await _repositories.Properties.Suggestions.ExecuteUpdateAsync(suggestion);
    }
}
