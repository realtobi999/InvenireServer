
using System.Text.Json;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Create;

public class CreatePropertySuggestionCommandHandler : IRequestHandler<CreatePropertySuggestionCommand, CreatePropertySuggestionCommandResult>
{
    private readonly IServiceManager _services;

    public CreatePropertySuggestionCommandHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task<CreatePropertySuggestionCommandResult> Handle(CreatePropertySuggestionCommand request, CancellationToken ct)
    {
        var employee = await _services.Employees.GetAsync(request.Jwt!);
        var organization = await _services.Organizations.TryGetForAsync(employee) ?? throw new BadRequest400Exception("You are not part of an organization.");
        var property = await _services.Properties.TryGetForAsync(organization) ?? throw new BadRequest400Exception("Organization you are part of doesn't have a property.");

        var suggestion = new PropertySuggestion
        {
            Id = request.Id ?? Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Feedback = null,
            PayloadString = JsonSerializer.Serialize(request.Payload),
            Status = PropertySuggestionStatus.PENDING,
            CreatedAt = DateTimeOffset.UtcNow,
            LastUpdatedAt = null,
            ResolvedAt = null,
            PropertyId = property.Id,
            EmployeeId = employee.Id,
        };

        await _services.Properties.Suggestion.CreateAsync(suggestion);

        return new CreatePropertySuggestionCommandResult
        {
            Suggestion = suggestion
        };
    }
}
