using FluentValidation;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Decline;

public class DeclinePropertySuggestionCommandValidator : AbstractValidator<DeclinePropertySuggestionCommand>
{
    public DeclinePropertySuggestionCommandValidator()
    {
        RuleFor(c => c.Feedback)
            .MaximumLength(PropertySuggestion.MAX_FEEDBACK_LENGTH)
            .WithName("feedback");
    }
}
