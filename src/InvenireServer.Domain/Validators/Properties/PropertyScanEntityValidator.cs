using FluentValidation.Results;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Domain.Validators.Properties;

public static class PropertyScanEntityValidator
{
    public static List<ValidationFailure> Validate(PropertyScan scan)
    {
        var errors = new List<ValidationFailure>();

        // Id.

        if (scan.Id == Guid.Empty)
            errors.Add(new ValidationFailure(nameof(scan.Id), "Id must not be empty."));

        // Name.

        if (string.IsNullOrWhiteSpace(scan.Name))
            errors.Add(new ValidationFailure(nameof(scan.Name), "Name must not be empty."));

        if (scan.Name != scan.Name.Trim())
            errors.Add(new ValidationFailure(nameof(scan.Name), "Name must not start or end with whitespace."));

        if (scan.Name.Length > PropertyScan.MAX_NAME_LENGTH)
            errors.Add(new ValidationFailure(nameof(scan.Name), $"Name must not exceed {PropertyScan.MAX_NAME_LENGTH} characters."));

        // Description.

        if (scan.Description is not null)
        {
            if (scan.Description != scan.Description.Trim())
                errors.Add(new ValidationFailure(nameof(scan.Description), "Description must not start or end with whitespace."));

            if (scan.Description.Length > PropertyScan.MAX_DESCRIPTION_LENGTH)
                errors.Add(new ValidationFailure(nameof(scan.Description), $"Description must not exceed {PropertyScan.MAX_DESCRIPTION_LENGTH} characters."));
        }

        // CreatedAt.

        if (scan.CreatedAt > DateTimeOffset.UtcNow)
            errors.Add(new ValidationFailure(nameof(scan.CreatedAt), "Creation date cannot be in the future."));

        if (scan.LastUpdatedAt.HasValue && scan.CreatedAt > scan.LastUpdatedAt.Value)
            errors.Add(new ValidationFailure(nameof(scan.CreatedAt), "Creation date cannot be after the last update date."));

        if (scan.ClosedAt.HasValue && scan.CreatedAt > scan.ClosedAt.Value)
            errors.Add(new ValidationFailure(nameof(scan.CreatedAt), "Creation date cannot be after the closing date."));

        // ClosedAt.

        if (scan.ClosedAt.HasValue && scan.ClosedAt > DateTimeOffset.UtcNow)
            errors.Add(new ValidationFailure(nameof(scan.ClosedAt), "Closing date cannot be in the future."));

        // LastUpdatedAt.

        if (scan.LastUpdatedAt.HasValue && scan.LastUpdatedAt > DateTimeOffset.UtcNow)
            errors.Add(new ValidationFailure(nameof(scan.LastUpdatedAt), "Last update date cannot be in the future."));


        // PropertyId.

        if (scan.PropertyId is null)
            errors.Add(new ValidationFailure(nameof(scan.PropertyId), $"Property must be assigned."));

        // Items.

        foreach (var item in scan.ScannedItems)
        {
            if (item.PropertyId is null || item.PropertyId != scan.PropertyId)
                errors.Add(new ValidationFailure(nameof(scan.ScannedItems), $"Scanned item (ID: {item.Id}) must belong to the assigned property. (ID: {scan.PropertyId})"));
        }

        return errors;
    }
}
