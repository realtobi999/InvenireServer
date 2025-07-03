using System.ComponentModel.DataAnnotations;

namespace InvenireServer.Application.Attributes;

public class PositiveDoubleAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is double d) return d >= 0;

        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be zero or a positive number.";
    }
}