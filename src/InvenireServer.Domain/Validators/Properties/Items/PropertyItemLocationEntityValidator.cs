using FluentValidation.Results;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Domain.Validators.Properties.Items;

public static class PropertyItemLocationEntityValidator
{
    public static List<ValidationFailure> Validate(PropertyItemLocation location)
    {
        var errors = new List<ValidationFailure>();

        // Room.

        if (string.IsNullOrWhiteSpace(location.Room))
            errors.Add(new ValidationFailure(nameof(location.Room), "Room must not be empty."));

        if (location.Room != location.Room.Trim())
            errors.Add(new ValidationFailure(nameof(location.Room), "Room must not start or end with whitespace."));

        if (location.Room.Length > PropertyItemLocation.MAX_ROOM_LENGTH)
            errors.Add(new ValidationFailure(nameof(location.Room), $"Room must not exceed {PropertyItemLocation.MAX_ROOM_LENGTH} characters."));

        // Building.

        if (string.IsNullOrWhiteSpace(location.Building))
            errors.Add(new ValidationFailure(nameof(location.Building), "Building must not be empty."));

        if (location.Building != location.Building.Trim())
            errors.Add(new ValidationFailure(nameof(location.Building), "Building must not start or end with whitespace."));

        if (location.Building.Length > PropertyItemLocation.MAX_BUILDING_LENGTH)
            errors.Add(new ValidationFailure(nameof(location.Building), $"Building must not exceed {PropertyItemLocation.MAX_BUILDING_LENGTH} characters."));

        // AdditionalNote.

        if (location.AdditionalNote is not null)
        {
            if (location.AdditionalNote != location.AdditionalNote.Trim())
                errors.Add(new ValidationFailure(nameof(location.AdditionalNote), "Additional note must not start or end with whitespace."));

            if (location.AdditionalNote.Length > PropertyItemLocation.MAX_ADDITIONAL_NOTE_LENGTH)
                errors.Add(new ValidationFailure(nameof(location.AdditionalNote), $"Additional note must not exceed {PropertyItemLocation.MAX_ROOM_LENGTH} characters."));
        }

        return errors;
    }
}
