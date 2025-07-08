using FluentValidation.Results;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Domain.Validators.Organizations;

public static class OrganizationInvitationEntityValidator
{
    public static List<ValidationFailure> Validate(OrganizationInvitation invitation)
    {
        var errors = new List<ValidationFailure>();

        // Id.

        if (invitation.Id == Guid.Empty)
            errors.Add(new(nameof(invitation.Id), "Id must not be empty."));

        // Description.

        if (invitation.Description is not null && invitation.Description.Length > Organization.MAX_NAME_LENGTH)
        {
            errors.Add(new(nameof(invitation.Description), $"Description must not exceed {Organization.MAX_NAME_LENGTH} characters."));
        }

        // CreatedAt.

        if (invitation.CreatedAt > DateTimeOffset.UtcNow)
            errors.Add(new(nameof(invitation.CreatedAt), "Creation date cannot be in the future."));

        if (invitation.LastUpdatedAt.HasValue && invitation.CreatedAt > invitation.LastUpdatedAt.Value)
            errors.Add(new(nameof(invitation.LastUpdatedAt), "Creation date cannot be after the last update date."));

        // LastUpdatedAt.

        if (invitation.LastUpdatedAt.HasValue && invitation.LastUpdatedAt > DateTimeOffset.UtcNow)
            errors.Add(new(nameof(invitation.LastUpdatedAt), "Last update date cannot be in the future."));

        // OrganizationId.

        if (invitation.OrganizationId.HasValue && invitation.OrganizationId == Guid.Empty)
            errors.Add(new(nameof(invitation.OrganizationId), "If organization is assigned, its ID must not be empty."));

        // Employee.

        if (invitation.Employee is null)
            errors.Add(new(nameof(invitation.Employee), $"Employee must be assigned to this invitation."));
        else
        {
            if (invitation.Employee!.OrganizationId is not null)
                errors.Add(new(nameof(invitation.Employee), $"Employee (ID: {invitation.Employee.Id}) must not belong to any organization."));

            if (!invitation.Employee!.IsVerified)
                errors.Add(new(nameof(invitation.Employee), $"Employee (ID: {invitation.Employee.Id}) must be verified."));
        }

        return errors;
    }
}
