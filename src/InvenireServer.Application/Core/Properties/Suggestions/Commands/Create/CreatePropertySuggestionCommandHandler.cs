using System.Text.Json;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Create;

public class CreatePropertySuggestionCommandHandler : IRequestHandler<CreatePropertySuggestionCommand, CreatePropertySuggestionCommandResult>
{
    private readonly IRepositoryManager _repositories;

    public CreatePropertySuggestionCommandHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<CreatePropertySuggestionCommandResult> Handle(CreatePropertySuggestionCommand request, CancellationToken ct)
    {
        var employee = await _repositories.Employees.GetAsync(request.Jwt!) ?? throw new NotFound404Exception("The employee was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(employee) ?? throw new BadRequest400Exception("The employee isn't part of any organization.");
        var property = await _repositories.Properties.GetForAsync(organization) ?? throw new BadRequest400Exception("The organization doesn't have a property.");

        var suggestion = new PropertySuggestion
        {
            Id = request.Id ?? Guid.NewGuid(),
            PropertyId = property.Id,
            EmployeeId = employee.Id,
            Name = request.Name,
            Description = request.Description,
            Feedback = null,
            PayloadString = JsonSerializer.Serialize(request.Payload),
            Status = PropertySuggestionStatus.PENDING,
            CreatedAt = DateTimeOffset.UtcNow,
            LastUpdatedAt = null,
            ResolvedAt = null,
        };

        await _repositories.Properties.Suggestions.ExecuteCreateAsync(suggestion);

        return new CreatePropertySuggestionCommandResult
        {
            Suggestion = suggestion
        };
    }
}
