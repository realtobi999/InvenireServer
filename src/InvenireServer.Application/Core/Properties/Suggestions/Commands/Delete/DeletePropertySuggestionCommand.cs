using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Delete;

/// <summary>
/// Represents a request to delete a property suggestion.
/// </summary>
public class DeletePropertySuggestionCommand : IRequest
{
    public required Jwt Jwt { get; set; }
    public required Guid SuggestionId { get; set; }
}
