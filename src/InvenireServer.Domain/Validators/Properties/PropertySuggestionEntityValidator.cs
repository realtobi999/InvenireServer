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

        // Status.

        if (!Enum.IsDefined(suggestion.Status))
            errors.Add(new ValidationFailure(nameof(suggestion.Status), "Status must be a valid enum value."));

        // CreatedAt.

        if (suggestion.CreatedAt > DateTimeOffset.UtcNow)
            errors.Add(new ValidationFailure(nameof(suggestion.CreatedAt), "Creation date cannot be in the future."));

        if (suggestion.LastUpdatedAt.HasValue && suggestion.CreatedAt > suggestion.LastUpdatedAt.Value)
            errors.Add(new ValidationFailure(nameof(suggestion.LastUpdatedAt), "Creation date cannot be after the last update date."));

        // LastUpdatedAt.

        if (suggestion.LastUpdatedAt.HasValue && suggestion.LastUpdatedAt > DateTimeOffset.UtcNow)
            errors.Add(new ValidationFailure(nameof(suggestion.LastUpdatedAt), "Last update date cannot be in the future."));

        // RequestBody.

        if (string.IsNullOrWhiteSpace(suggestion.RequestBody))
            errors.Add(new ValidationFailure(nameof(suggestion.RequestBody), "Request body must not be empty."));

        if (suggestion.RequestBody != suggestion.RequestBody.Trim())
            errors.Add(new ValidationFailure(nameof(suggestion.RequestBody), "Request body must not start or end with whitespace."));

        // RequestType.

        if (!Enum.IsDefined(suggestion.RequestType))
            errors.Add(new ValidationFailure(nameof(suggestion.RequestType), "Request type must be a valid enum value."));

        return errors;
    }
}
