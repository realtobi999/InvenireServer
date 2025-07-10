using FluentValidation.Results;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Domain.Validators.Properties;

public static class PropertyItemEntityValidator
{
    public static List<ValidationFailure> Validate(PropertyItem item)
    {
        var errors = new List<ValidationFailure>();

        // Id.

        if (item.Id == Guid.Empty)
            errors.Add(new ValidationFailure(nameof(item.Id), "Id must not be empty."));

        // InventoryNumber.

        if (string.IsNullOrWhiteSpace(item.InventoryNumber))
            errors.Add(new ValidationFailure(nameof(item.InventoryNumber), "Inventory number must not be empty."));

        if (item.InventoryNumber != item.InventoryNumber.Trim())
            errors.Add(new ValidationFailure(nameof(item.InventoryNumber), "Inventory number must not start or end with whitespace."));

        if (item.InventoryNumber.Length > PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH)
            errors.Add(new ValidationFailure(nameof(item.InventoryNumber), $"Inventory number must not exceed {PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH} characters."));

        // RegistrationNumber.

        if (string.IsNullOrWhiteSpace(item.RegistrationNumber))
            errors.Add(new ValidationFailure(nameof(item.RegistrationNumber), "Registration number must not be empty."));

        if (item.RegistrationNumber != item.RegistrationNumber.Trim())
            errors.Add(new ValidationFailure(nameof(item.RegistrationNumber), "Registration number must not start or end with whitespace."));

        if (item.RegistrationNumber.Length > PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH)
            errors.Add(new ValidationFailure(nameof(item.RegistrationNumber), $"Registration number must not exceed {PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH} characters."));

        // Name.

        if (string.IsNullOrWhiteSpace(item.Name))
            errors.Add(new ValidationFailure(nameof(item.Name), "Name must not be empty."));

        if (item.Name != item.Name.Trim())
            errors.Add(new ValidationFailure(nameof(item.Name), "Name must not start or end with whitespace."));

        if (item.Name.Length > PropertyItem.MAX_NAME_LENGTH)
            errors.Add(new ValidationFailure(nameof(item.Name), $"Name must not exceed {PropertyItem.MAX_NAME_LENGTH} characters."));

        // Price.

        if (item.Price < 0)
            errors.Add(new ValidationFailure(nameof(item.Price), "Price must not be negative."));

        // SerialNumber.

        if (item.SerialNumber is not null)
        {
            if (item.SerialNumber != item.SerialNumber.Trim())
                errors.Add(new ValidationFailure(nameof(item.SerialNumber), "Serial number must not start or end with whitespace."));

            if (item.SerialNumber.Length > PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH)
                errors.Add(new ValidationFailure(nameof(item.SerialNumber), $"Serial number must not exceed {PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH} characters."));
        }

        // DateOfPurchase.

        if (item.DateOfPurchase > DateTimeOffset.UtcNow)
            errors.Add(new ValidationFailure(nameof(item.DateOfPurchase), "Date of purchase cannot be in the future."));

        if (item.DateOfSale.HasValue && item.DateOfPurchase > item.DateOfSale.Value)
            errors.Add(new ValidationFailure(nameof(item.DateOfPurchase), "Date of purchase cannot be after the date of sale."));

        // DateOfSale.

        if (item.DateOfSale.HasValue && item.DateOfSale > DateTimeOffset.UtcNow)
            errors.Add(new ValidationFailure(nameof(item.DateOfSale), "Date of sale cannot be in the future."));

        // Description.

        if (item.Description is not null)
        {
            if (item.Description != item.Description.Trim())
                errors.Add(new ValidationFailure(nameof(item.Description), "Description must not start or end with whitespace."));

            if (item.Description.Length > PropertyItem.MAX_DESCRIPTION_LENGTH)
                errors.Add(new ValidationFailure(nameof(item.Description), $"Description must not exceed {PropertyItem.MAX_DESCRIPTION_LENGTH} characters."));
        }

        // DocumentNumber.

        if (string.IsNullOrWhiteSpace(item.DocumentNumber))
            errors.Add(new ValidationFailure(nameof(item.DocumentNumber), "Document number must not be empty."));

        if (item.DocumentNumber != item.DocumentNumber.Trim())
            errors.Add(new ValidationFailure(nameof(item.DocumentNumber), "Document number must not start or end with whitespace."));

        if (item.DocumentNumber.Length > PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH)
            errors.Add(new ValidationFailure(nameof(item.DocumentNumber), $"Document number must not exceed {PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH} characters."));


        // CreatedAt.

        if (item.CreatedAt > DateTimeOffset.UtcNow)
            errors.Add(new ValidationFailure(nameof(item.CreatedAt), "Creation date cannot be in the future."));

        if (item.LastUpdatedAt.HasValue && item.CreatedAt > item.LastUpdatedAt.Value)
            errors.Add(new ValidationFailure(nameof(item.LastUpdatedAt), "Creation date cannot be after the last update date."));

        // LastUpdatedAt.

        if (item.LastUpdatedAt.HasValue && item.LastUpdatedAt > DateTimeOffset.UtcNow)
            errors.Add(new ValidationFailure(nameof(item.LastUpdatedAt), "Last update date cannot be in the future."));


        // PropertyId.

        if (item.PropertyId is null)
            errors.Add(new ValidationFailure(nameof(item.PropertyId), $"Property must be assigned."));

        return errors;
    }
}
