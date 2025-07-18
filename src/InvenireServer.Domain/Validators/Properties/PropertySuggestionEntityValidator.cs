using FluentValidation.Results;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Domain.Validators.Properties;

public static class PropertySuggestionEntityValidator
{
    public static List<ValidationFailure> Validate(PropertySuggestion suggestion)
    {
        var errors = new List<ValidationFailure>();

        // Id.

        if (suggestion.Id == Guid.Empty)
            errors.Add(new ValidationFailure(nameof(suggestion.Id), "Id must not be empty."));

        // Name.

        if (string.IsNullOrWhiteSpace(suggestion.Name))
            errors.Add(new ValidationFailure(nameof(suggestion.Name), "Name must not be empty."));

        if (suggestion.Name != suggestion.Name.Trim())
            errors.Add(new ValidationFailure(nameof(suggestion.Name), "Name must not start or end with whitespace."));

        if (suggestion.Name.Length > PropertySuggestion.MAX_NAME_LENGTH)
            errors.Add(new ValidationFailure(nameof(suggestion.Name), $"Name must not exceed {PropertySuggestion.MAX_NAME_LENGTH} characters."));

        // Description.

        if (suggestion.Description is not null)
        {
            if (suggestion.Description != suggestion.Description.Trim())
                errors.Add(new ValidationFailure(nameof(suggestion.Description), "Description must not start or end with whitespace."));

            if (suggestion.Description.Length > PropertySuggestion.MAX_DESCRIPTION_LENGTH)
                errors.Add(new ValidationFailure(nameof(suggestion.Description), $"Description must not exceed {PropertySuggestion.MAX_DESCRIPTION_LENGTH} characters."));
        }

        // Feedback.

        if (suggestion.Feedback is not null)
        {
            if (suggestion.Feedback != suggestion.Feedback.Trim())
                errors.Add(new ValidationFailure(nameof(suggestion.Feedback), "Feedback must not start or end with whitespace."));

            if (suggestion.Feedback.Length > PropertySuggestion.MAX_DESCRIPTION_LENGTH)
                errors.Add(new ValidationFailure(nameof(suggestion.Description), $"Feedback must not exceed {PropertySuggestion.MAX_FEEDBACK_LENGTH} characters."));
        }

        // PayloadString.

        if (string.IsNullOrWhiteSpace(suggestion.PayloadString))
            errors.Add(new ValidationFailure(nameof(suggestion.PayloadString), "Payload string must not be empty."));

        if (suggestion.PayloadString != suggestion.PayloadString.Trim())
            errors.Add(new ValidationFailure(nameof(suggestion.PayloadString), "Payload string must not start or end with whitespace."));

        // Status.

        if (!Enum.IsDefined(suggestion.Status))
            errors.Add(new ValidationFailure(nameof(suggestion.Status), "Status must be a valid enum value."));

        // CreatedAt.

        if (suggestion.CreatedAt > DateTimeOffset.UtcNow)
            errors.Add(new ValidationFailure(nameof(suggestion.CreatedAt), "Creation date cannot be in the future."));

        if (suggestion.LastUpdatedAt.HasValue && suggestion.CreatedAt > suggestion.LastUpdatedAt.Value)
            errors.Add(new ValidationFailure(nameof(suggestion.LastUpdatedAt), "Creation date cannot be after the last update date."));

        if (suggestion.ResolvedAt.HasValue && suggestion.CreatedAt > suggestion.ResolvedAt.Value)
            errors.Add(new ValidationFailure(nameof(suggestion.ResolvedAt), "Creation date cannot be after resolved at date."));

        // ResolvedAt.

        if (suggestion.ResolvedAt.HasValue)
        {
            if (suggestion.ResolvedAt > DateTimeOffset.UtcNow)
                errors.Add(new ValidationFailure(nameof(suggestion.ResolvedAt), "Resolved at date cannot be in the future."));

            if (suggestion.Status == PropertySuggestionStatus.PENDING)
                errors.Add(new ValidationFailure(nameof(suggestion.ResolvedAt), $"Resolved at date cannot be set if the status is '{PropertySuggestionStatus.PENDING}'."));
        }

        // LastUpdatedAt.

        if (suggestion.LastUpdatedAt.HasValue && suggestion.LastUpdatedAt > DateTimeOffset.UtcNow)
            errors.Add(new ValidationFailure(nameof(suggestion.LastUpdatedAt), "Last update date cannot be in the future."));

        // EmployeeId.

        if (suggestion.EmployeeId is null)
            errors.Add(new ValidationFailure(nameof(suggestion.EmployeeId), $"Employee must be assigned."));

        // PropertyId.

        if (suggestion.PropertyId is null)
            errors.Add(new ValidationFailure(nameof(suggestion.PropertyId), $"Property must be assigned."));

        return errors;
    }
}
