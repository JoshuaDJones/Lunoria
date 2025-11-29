using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

public class RequiredIfMissingAttribute : ValidationAttribute
{
    private readonly string _otherProperty;

    public RequiredIfMissingAttribute(string otherProperty)
    {
        _otherProperty = otherProperty;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var otherProp = validationContext.ObjectType.GetProperty(_otherProperty);

        if (otherProp == null)
            return new ValidationResult($"Unknown property: {_otherProperty}");

        var otherValue = otherProp.GetValue(validationContext.ObjectInstance);

        bool otherMissing =
            otherValue == null ||
            (otherValue is string s && string.IsNullOrWhiteSpace(s)) ||
            (otherValue is IBrowserFile f && f == null);

        bool thisMissing =
            value == null ||
            (value is string s2 && string.IsNullOrWhiteSpace(s2)) ||
            (value is IBrowserFile f2 && f2 == null);

        if (otherMissing && thisMissing)
        {
            return new ValidationResult(
                ErrorMessage,
                new[] { validationContext.MemberName! }
            );
        }

        return ValidationResult.Success;
    }
}
