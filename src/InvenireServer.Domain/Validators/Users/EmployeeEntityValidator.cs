using FluentValidation.Results;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Domain.Validators.Users;

public static class EmployeeEntityValidator
{
    public static List<ValidationFailure> Validate(Employee employee)
    {
        var errors = new List<ValidationFailure>();

        // Id.

        if (employee.Id == Guid.Empty)
            errors.Add(new ValidationFailure(nameof(employee.Id), "Id must not be empty."));

        // Name.

        if (string.IsNullOrWhiteSpace(employee.Name))
            errors.Add(new ValidationFailure(nameof(employee.Name), "Name must not be empty."));

        if (employee.Name != employee.Name.Trim())
            errors.Add(new ValidationFailure(nameof(employee.Name), "Name must not start or end with whitespace."));

        if (employee.Name.Length > Employee.MAX_NAME_LENGTH)
            errors.Add(new ValidationFailure(nameof(employee.Name), $"Name must not exceed {Employee.MAX_NAME_LENGTH} characters."));

        // EmailAddress.

        if (string.IsNullOrWhiteSpace(employee.EmailAddress))
            errors.Add(new ValidationFailure(nameof(employee.EmailAddress), "Email address must not be empty."));

        if (employee.EmailAddress != employee.EmailAddress.Trim())
            errors.Add(new ValidationFailure(nameof(employee.EmailAddress), "Email address must not start or end with whitespace."));

        if (employee.EmailAddress.Length > Employee.MAX_EMAIL_ADDRESS_LENGTH)
            errors.Add(new ValidationFailure(nameof(employee.EmailAddress), $"Email address must not exceed {Employee.MAX_EMAIL_ADDRESS_LENGTH} characters."));

        if (!employee.EmailAddress.Contains('@') || !employee.EmailAddress.Contains('.'))
            errors.Add(new ValidationFailure(nameof(employee.EmailAddress), "Email address must be a valid format."));

        // Password (hashed).

        if (string.IsNullOrWhiteSpace(employee.Password))
            errors.Add(new ValidationFailure(nameof(employee.Password), "Password must not be empty."));

        // CreatedAt.

        if (employee.CreatedAt > DateTimeOffset.UtcNow)
            errors.Add(new ValidationFailure(nameof(employee.CreatedAt), "Creation date cannot be in the future."));

        if (employee.LastUpdatedAt.HasValue && employee.CreatedAt > employee.LastUpdatedAt.Value)
            errors.Add(new ValidationFailure(nameof(employee.LastUpdatedAt), "Creation date cannot be after the last update date."));

        if (employee.LastLoginAt.HasValue && employee.CreatedAt > employee.LastLoginAt.Value)
            errors.Add(new ValidationFailure(nameof(employee.LastLoginAt), "Creation date cannot be after the last login date."));

        // LastUpdatedAt.

        if (employee.LastUpdatedAt.HasValue && employee.LastUpdatedAt > DateTimeOffset.UtcNow)
            errors.Add(new ValidationFailure(nameof(employee.LastUpdatedAt), "Last update date cannot be in the future."));

        // LastLoginAt.

        if (employee.LastLoginAt.HasValue && employee.LastLoginAt > DateTimeOffset.UtcNow)
            errors.Add(new ValidationFailure(nameof(employee.LastLoginAt), "Last login date cannot be in the future."));

        // OrganizationId.

        if (employee.OrganizationId.HasValue && employee.OrganizationId == Guid.Empty)
            errors.Add(new ValidationFailure(nameof(employee.OrganizationId), "If organization is assigned, its ID must not be empty."));

        // AssignedItems.

        foreach (var item in employee.AssignedItems)
        {
            if (item.EmployeeId.HasValue && item.EmployeeId != employee.Id)
            {
                errors.Add(new ValidationFailure(nameof(employee.AssignedItems), $"Assigned item (ID: {item.Id}) belongs to a different employee."));
            }
        }


        return errors;
    }
}
