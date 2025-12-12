using FluentValidation;
using InvenireServer.Application.Interfaces.Common;

namespace InvenireServer.Application.Core.Properties.Items.Commands.GenerateCodes;

public class GenerateCodesPropertyItemsCommandValidator : AbstractValidator<GenerateCodesPropertyItemsCommand>
{
    public GenerateCodesPropertyItemsCommandValidator()
    {
        RuleFor(c => c.Size)
            .NotEmpty()
            .LessThanOrEqualTo(IQuickResponseCodeGenerator.MaximumSize)
            .GreaterThanOrEqualTo(IQuickResponseCodeGenerator.MinimumSize)
            .WithName("size");
    }
}
