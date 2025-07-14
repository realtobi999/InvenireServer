using FluentValidation;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Create;

public class CreatePropertySuggestionCommandValidator : AbstractValidator<CreatePropertySuggestionCommand>
{
    public CreatePropertySuggestionCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(PropertySuggestion.MAX_NAME_LENGTH)
            .WithName("name");
        RuleFor(c => c.Description)
            .MaximumLength(PropertySuggestion.MAX_DESCRIPTION_LENGTH)
            .WithName("description");
        RuleFor(c => c.RequestBody)
            .NotEmpty()
            .WithName("request_body");
    }
}
