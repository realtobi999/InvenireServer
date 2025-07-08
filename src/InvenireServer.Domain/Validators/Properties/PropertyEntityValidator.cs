using FluentValidation.Results;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Domain.Validators.Properties;

public static class PropertyEntityValidator
{
    public static List<ValidationFailure> Validate(Property property)
    {
        var errors = new List<ValidationFailure>();

        // Id.

        if (property.Id == Guid.Empty)
            errors.Add(new(nameof(property.Id), "Id must not be empty."));

        // CreatedAt.

        if (property.CreatedAt > DateTimeOffset.UtcNow)
            errors.Add(new(nameof(property.CreatedAt), "Creation date cannot be in the future."));

        if (property.LastUpdatedAt.HasValue && property.CreatedAt > property.LastUpdatedAt.Value)
            errors.Add(new(nameof(property.LastUpdatedAt), "Creation date cannot be after the last update date."));

        // LastUpdatedAt.

        if (property.LastUpdatedAt.HasValue && property.LastUpdatedAt > DateTimeOffset.UtcNow)
            errors.Add(new(nameof(property.LastUpdatedAt), "Last update date cannot be in the future."));

        // OrganizationId.

        if (property.OrganizationId is null)
            errors.Add(new(nameof(property.OrganizationId), $"Organization must be assigned."));

        // Items.

        foreach (var item in property.Items)
        {
            if (item.PropertyId is null || item.PropertyId != property.Id)
                errors.Add(new(nameof(property.Items), $"Item (ID: {item.Id}) must belong to this property."));
        }

        return errors;
    }
}
