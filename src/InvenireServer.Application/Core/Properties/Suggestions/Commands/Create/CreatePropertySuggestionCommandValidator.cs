using FluentValidation;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Application.Core.Properties.Items.Commands.Update;
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
        RuleFor(c => c.Payload)
            .NotEmpty()
            .WithName("payload")
            .ChildRules(body =>
            {
                body.RuleForEach(b => b.CreateCommands).SetValidator(new CreatePropertyItemCommandValidator());
                body.RuleForEach(b => b.UpdateCommands).SetValidator(new UpdatePropertyItemCommandValidator());
            });
    }
}
