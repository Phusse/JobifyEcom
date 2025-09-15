using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Validation;

/// <summary>
/// Ensures that all items in a list are non-null, non-empty, and not whitespace-only.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class AllItemsRequiredAttribute : ValidationAttribute
{
	/// <inheritdoc/>
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if (value is not List<string> items)
			return new ValidationResult("Invalid string list.");

		var invalidItems = items.Where(t => string.IsNullOrWhiteSpace(t)).ToList();

		if (invalidItems.Count != 0)
			return new ValidationResult(ErrorMessage ?? "All items must be non-empty and cannot contain only whitespace.");

		return ValidationResult.Success;
	}
}
