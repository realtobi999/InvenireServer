using System.ComponentModel.DataAnnotations;

namespace InvenireServer.Domain.Core.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class SameAsAttribute : ValidationAttribute
{
    private readonly string _cProperty;

    public SameAsAttribute(string cProperty)
    {
        _cProperty = cProperty;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        // get the value of the property to compare
        var property = context.ObjectType.GetProperty(_cProperty);

        if (property == null)
        {
            return new ValidationResult($"Property '{_cProperty}' not found.");
        }

        var cValue = property.GetValue(context.ObjectInstance);

        // compare the values
        if (!Equals(value, cValue))
        {
            return new ValidationResult($"The {context.DisplayName} and {_cProperty} fields do not match.");
        }

        return ValidationResult.Success;
    }
}