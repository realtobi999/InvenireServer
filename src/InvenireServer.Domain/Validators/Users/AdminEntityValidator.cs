using FluentValidation.Results;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Domain.Validators.Users;

public static class AdminEntityValidator
{
    public static List<ValidationFailure> Validate(Admin admin)
    {
        var errors = new List<ValidationFailure>();

        // Id.

        if (admin.Id == Guid.Empty)
            errors.Add(new(nameof(admin.Id), "Id must not be empty."));

        // Name.

        if (string.IsNullOrWhiteSpace(admin.Name))
            errors.Add(new(nameof(admin.Name), "Name must not be empty."));

        if (admin.Name != admin.Name.Trim())
            errors.Add(new(nameof(admin.Name), "Name must not start or end with whitespace."));

        if (admin.Name.Length > Admin.MAX_NAME_LENGTH)
            errors.Add(new(nameof(admin.Name), $"Name must not exceed {Admin.MAX_NAME_LENGTH} characters."));

        // Email Address.

        if (string.IsNullOrWhiteSpace(admin.EmailAddress))
            errors.Add(new(nameof(admin.EmailAddress), "Email address must not be empty."));

        if (admin.EmailAddress != admin.EmailAddress.Trim())
            errors.Add(new(nameof(admin.EmailAddress), "Email address must not start or end with whitespace."));

        if (admin.EmailAddress.Length > Admin.MAX_EMAIL_ADDRESS_LENGTH)
            errors.Add(new(nameof(admin.EmailAddress), $"Email address must not exceed {Admin.MAX_EMAIL_ADDRESS_LENGTH} characters."));

        if (!admin.EmailAddress.Contains('@') || !admin.EmailAddress.Contains('.'))
            errors.Add(new(nameof(admin.EmailAddress), "Email address must be a valid format."));

        // Password (hashed).

        if (string.IsNullOrWhiteSpace(admin.Password))
            errors.Add(new(nameof(admin.Password), "Password must not be empty."));

        // CreatedAt.

        if (admin.CreatedAt > DateTimeOffset.UtcNow)
            errors.Add(new(nameof(admin.CreatedAt), "Creation date cannot be in the future."));

        if (admin.LastUpdatedAt.HasValue && admin.CreatedAt > admin.LastUpdatedAt.Value)
            errors.Add(new(nameof(admin.LastUpdatedAt), "Creation date cannot be after the last update date."));

        if (admin.LastLoginAt.HasValue && admin.CreatedAt > admin.LastLoginAt.Value)
            errors.Add(new(nameof(admin.LastLoginAt), "Creation date cannot be after the last login date."));

        // LastUpdatedAt.

        if (admin.LastUpdatedAt.HasValue && admin.LastUpdatedAt > DateTimeOffset.UtcNow)
            errors.Add(new(nameof(admin.LastUpdatedAt), "Last update date cannot be in the future."));

        // LastLoginAt.

        if (admin.LastLoginAt.HasValue && admin.LastLoginAt > DateTimeOffset.UtcNow)
            errors.Add(new(nameof(admin.LastLoginAt), "Last login date cannot be in the future."));

        // OrganizationId.

        if (admin.OrganizationId.HasValue && admin.OrganizationId == Guid.Empty)
            errors.Add(new(nameof(admin.OrganizationId), "If organization is assigned, its ID must not be empty."));

        return errors;
    }
}
