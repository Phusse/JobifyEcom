using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Validation;

/// <summary>
/// Ensures that a list of strings contains at least one non-null, non-empty, non-whitespace string.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class NonEmptyListAttribute : ValidationAttribute
{
	/// <inheritdoc/>
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if (value is not List<string> items)
			return new ValidationResult("Invalid string list.");

		if (!items.Any(t => !string.IsNullOrWhiteSpace(t)))
			return new ValidationResult(ErrorMessage ?? "At least one non-empty item is required.");

		return ValidationResult.Success;
	}
}
