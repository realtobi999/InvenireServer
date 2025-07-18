using FluentValidation;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Application.Core.Properties.Items.Commands.Update;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Core.Properties.Suggestions.Commands.Update;

public class UpdatePropertySuggestionCommandValidator : AbstractValidator<UpdatePropertySuggestionCommand>
{
    public UpdatePropertySuggestionCommandValidator()
    {
        RuleFor(c => c.Description)
            .MaximumLength(PropertySuggestion.MAX_DESCRIPTION_LENGTH)
            .WithName("description");
        When(c => c.Body != null, () =>
        {
            RuleForEach(c => c.Body!.CreateCommands).SetValidator(new CreatePropertyItemCommandValidator());
            RuleForEach(c => c.Body!.UpdateCommands).SetValidator(new UpdatePropertyItemCommandValidator());
        });
    }
}
