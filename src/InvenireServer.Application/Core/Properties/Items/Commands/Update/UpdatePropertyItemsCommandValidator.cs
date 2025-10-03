using FluentValidation;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Core.Properties.Items.Commands.Update;

public class UpdatePropertyItemsCommandValidator : AbstractValidator<UpdatePropertyItemsCommand>
{
    public UpdatePropertyItemsCommandValidator()
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
                    var result = new UpdatePropertyItemCommandValidator().Validate(item);

                    if (!result.IsValid)
                        foreach (var error in result.Errors)
                            context.AddFailure($"Item {(string.IsNullOrEmpty(item.InventoryNumber) ? "MISSING_INVENTORY_NUMBER" : item.InventoryNumber)}: {error.ErrorMessage}");
                }
            });
    }
}

public class UpdatePropertyItemCommandValidator : AbstractValidator<UpdatePropertyItemCommand>
{
    public UpdatePropertyItemCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
            .WithName("id");
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