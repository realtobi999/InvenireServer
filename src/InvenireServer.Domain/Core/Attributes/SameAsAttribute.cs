using System.ComponentModel.DataAnnotations;

namespace InvenireServer.Domain.Core.Attributes;


/// <summary>
/// Validation attribute that ensures the value of a property is the same as another specified property.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SameAsAttribute : ValidationAttribute
{
    private readonly string _cProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="SameAsAttribute"/> class.
    /// </summary>
    /// <param name="cProperty">The name of the property to compare against.</param>
    public SameAsAttribute(string cProperty)
    {
        _cProperty = cProperty;
    }

    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        // Get the value of the property to compare.
        var property = context.ObjectType.GetProperty(_cProperty);

        if (property == null)
        {
            return new ValidationResult($"Property '{_cProperty}' not found.");
        }

        var cValue = property.GetValue(context.ObjectInstance);

        // Compare the values.
        if (!Equals(value, cValue))
        {
            return new ValidationResult($"The {context.DisplayName} and {_cProperty} fields do not match.");
        }

        return ValidationResult.Success;
    }
}
