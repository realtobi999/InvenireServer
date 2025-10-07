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
        RuleFor(c => c.Payload)
            .NotEmpty()
            .WithName("payload")
            .ChildRules(payload =>
            {
                payload.RuleFor(p => p.UpdateCommands)
                    .Custom((commands, context) =>
                    {
                        if (commands != null && commands.Count != 0)
                        {
                            var result = new UpdatePropertyItemsCommandValidator().Validate(new UpdatePropertyItemsCommand
                            {
                                Items = commands
                            });

                            if (!result.IsValid)
                            {
                                foreach (var error in result.Errors)
                                {
                                    context.AddFailure(error);
                                }
                            }
                        }
                    });
            });
    }
}
