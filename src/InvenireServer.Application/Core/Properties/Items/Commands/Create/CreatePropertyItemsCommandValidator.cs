using FluentValidation;
using InvenireServer.Domain.Entities.Properties;
using Microsoft.IdentityModel.Tokens;

namespace InvenireServer.Application.Core.Properties.Items.Commands.Create;

/// <summary>
/// Defines validation rules for creating property items.
/// </summary>
public class CreatePropertyItemsCommandValidator : AbstractValidator<CreatePropertyItemsCommand>
{
    public CreatePropertyItemsCommandValidator()
    {
        RuleFor(c => c.Items)
            .NotEmpty()
            .WithName("items");

        RuleFor(c => c.Items)
            .Custom((items, context) =>
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var result = new CreatePropertyItemCommandValidator().Validate(item);

                    if (!result.IsValid)
                        foreach (var error in result.Errors)
                            context.AddFailure($"Item {(string.IsNullOrEmpty(item.InventoryNumber) ? "MISSING_INVENTORY_NUMBER" : item.InventoryNumber)}: {error.ErrorMessage}");
                }
            });
    }
}

/// <summary>
/// Defines validation rules for creating a property item.
/// </summary>
public class CreatePropertyItemCommandValidator : AbstractValidator<CreatePropertyItemCommand>
{
    public CreatePropertyItemCommandValidator()
    {
        RuleFor(c => c.InventoryNumber)
            .NotEmpty()
            .MaximumLength(PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH)
            .WithName("inventory_number");
        RuleFor(c => c.RegistrationNumber)
            .NotEmpty()
            .MaximumLength(PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH)
            .WithName("registration_number");
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(PropertyItem.MAX_NAME_LENGTH)
            .WithName("name");
        RuleFor(c => c.Price)
            .GreaterThanOrEqualTo(0)
            .WithName("price");
        RuleFor(c => c.SerialNumber)
            .MaximumLength(PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH)
            .WithName("serial_number");
        RuleFor(c => c.DateOfPurchase)
            .NotEmpty()
            .NotEqual(DateTimeOffset.MinValue)
            .WithName("date_of_purchase");
        RuleFor(c => c.Location)
            .NotEmpty()
            .WithName("location")
            .ChildRules(location =>
            {
                location.RuleFor(l => l.Room).NotEmpty().MaximumLength(PropertyItemLocation.MAX_ROOM_LENGTH).WithName("room");
                location.RuleFor(l => l.Building).NotEmpty().MaximumLength(PropertyItemLocation.MAX_BUILDING_LENGTH).WithName("building");
                location.RuleFor(l => l.AdditionalNote).MaximumLength(PropertyItemLocation.MAX_ADDITIONAL_NOTE_LENGTH).WithName("additional_note");
            });
        RuleFor(c => c.Description)
            .MaximumLength(PropertyItem.MAX_DESCRIPTION_LENGTH)
            .WithName("description");
        RuleFor(c => c.DocumentNumber)
            .MaximumLength(PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH)
            .WithName("document_number");
    }
}