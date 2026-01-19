using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Accept;

/// <summary>
/// Represents a request to accept a property suggestion.
/// </summary>
public record AcceptPropertySuggestionCommand : IRequest
{
    public required Jwt Jwt { get; init; }
    public required Guid SuggestionId { get; init; }
}
