using FluentValidation.Results;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Domain.Validators.Organizations;

public static class OrganizationEntityValidator
{
    public static List<ValidationFailure> Validate(Organization organization)
    {
        var errors = new List<ValidationFailure>();

        // Id.

        if (organization.Id == Guid.Empty)
            errors.Add(new ValidationFailure(nameof(organization.Id), "Id must not be empty."));

        // Name.

        if (string.IsNullOrWhiteSpace(organization.Name))
            errors.Add(new ValidationFailure(nameof(organization.Name), "Name must not be empty."));

        if (organization.Name != organization.Name.Trim())
            errors.Add(new ValidationFailure(nameof(organization.Name), "Name must not start or end with whitespace."));

        if (organization.Name.Length > Organization.MAX_NAME_LENGTH)
            errors.Add(new ValidationFailure(nameof(organization.Name), $"Name must not exceed {Organization.MAX_NAME_LENGTH} characters."));

        // CreatedAt.

        if (organization.CreatedAt > DateTimeOffset.UtcNow)
            errors.Add(new ValidationFailure(nameof(organization.CreatedAt), "Creation date cannot be in the future."));

        if (organization.LastUpdatedAt.HasValue && organization.CreatedAt > organization.LastUpdatedAt.Value)
            errors.Add(new ValidationFailure(nameof(organization.LastUpdatedAt), "Creation date cannot be after the last update date."));

        // LastUpdatedAt.

        if (organization.LastUpdatedAt.HasValue && organization.LastUpdatedAt > DateTimeOffset.UtcNow)
            errors.Add(new ValidationFailure(nameof(organization.LastUpdatedAt), "Last update date cannot be in the future."));

        // Admin.

        if (organization.Admin is null)
            errors.Add(new ValidationFailure(nameof(organization.Admin), $"Admin must be assigned to this organization."));
        else
        {
            if (organization.Admin!.OrganizationId != organization.Id)
                errors.Add(new ValidationFailure(nameof(organization.Admin), $"Admin (ID: {organization.Admin.Id}) must belong to this organization."));

            if (!organization.Admin!.IsVerified)
                errors.Add(new ValidationFailure(nameof(organization.Admin), $"Admin (ID: {organization.Admin.Id}) must be verified."));
        }

        // Property.

        if (organization.Property is not null && organization.Property.OrganizationId != organization.Id)
            errors.Add(new ValidationFailure(nameof(organization.Property), "Property must belong to this organization."));

        // Employees.

        if (organization.Employees.GroupBy(i => i.EmailAddress).Where(g => g.Count() > 1).ToList().Count != 0)
            errors.Add(new ValidationFailure(nameof(organization.Invitations), "There are duplicate employees with the same email address."));

        foreach (var employee in organization.Employees)
        {
            if (employee.OrganizationId is null || employee.OrganizationId != organization.Id)
                errors.Add(new ValidationFailure(nameof(organization.Employees), $"Employee (ID: {employee.Id}) must belong to this organization."));

            if (!employee.IsVerified)
                errors.Add(new ValidationFailure(nameof(organization.Employees), $"Employee (ID: {employee.Id}) must be verified."));
        }

        // Invitations.

        if (organization.Invitations.Count > Organization.MAX_INVITATIONS)
            errors.Add(new ValidationFailure(nameof(organization.Invitations), $"Total invitations must not exceed {Organization.MAX_INVITATIONS}."));

        if (organization.Invitations.GroupBy(i => (i.Employee!.Id, i.OrganizationId)).Where(g => g.Count() > 1).ToList().Count != 0)
            errors.Add(new ValidationFailure(nameof(organization.Invitations), "There are duplicate invitations for the same employee."));

        foreach (var invitation in organization.Invitations)
        {
            if (invitation.OrganizationId is null || invitation.OrganizationId != organization.Id)
                errors.Add(new ValidationFailure(nameof(organization.Invitations), $"Invitation (ID: {invitation.Id}) must belong to this organization."));
        }

        return errors;
    }
}
