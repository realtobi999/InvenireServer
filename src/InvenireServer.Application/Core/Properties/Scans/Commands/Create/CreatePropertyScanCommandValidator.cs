using FluentValidation;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Core.Properties.Scans.Commands.Create;

/// <summary>
/// Defines validation rules for creating a property scan.
/// </summary>
public class CreatePropertyScanCommandValidator : AbstractValidator<CreatePropertyScanCommand>
{
    public CreatePropertyScanCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(PropertyScan.MAX_NAME_LENGTH)
            .WithName("name");
        RuleFor(c => c.Description)
            .MaximumLength(PropertyScan.MAX_DESCRIPTION_LENGTH)
            .WithName("description");
    }
}
