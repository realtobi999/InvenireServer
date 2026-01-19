using FluentValidation;
using InvenireServer.Application.Interfaces.Common;

namespace InvenireServer.Application.Core.Properties.Items.Commands.GenerateCodes;

/// <summary>
/// Defines validation rules for generating codes for property items.
/// </summary>
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
