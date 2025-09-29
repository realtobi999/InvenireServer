using FluentValidation;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Core.Properties.Scans.Commands.Update;

public class UpdatePropertyScanCommandValidator : AbstractValidator<UpdatePropertyScanCommand>
{
    public UpdatePropertyScanCommandValidator()
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
