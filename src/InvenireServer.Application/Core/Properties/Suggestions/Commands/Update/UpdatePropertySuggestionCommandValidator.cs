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
        When(c => c.Payload != null, () =>
        {
            RuleForEach(c => c.Payload!.CreateCommands).SetValidator(new CreatePropertyItemCommandValidator());
            RuleForEach(c => c.Payload!.UpdateCommands).SetValidator(new UpdatePropertyItemCommandValidator());
        });
    }
}
