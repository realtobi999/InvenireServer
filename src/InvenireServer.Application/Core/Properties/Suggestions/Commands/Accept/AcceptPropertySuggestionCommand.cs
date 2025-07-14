using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Accept;

public class AcceptPropertySuggestionCommand : IRequest
{
    public required Guid SuggestionId { get; set; }

    public required Jwt Jwt { get; set; }
}
