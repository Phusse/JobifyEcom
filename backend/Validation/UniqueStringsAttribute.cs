using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Validation;

/// <summary>
/// Ensures that all strings in a list are unique, ignoring case.
/// <para>Trims whitespace and ignores empty or null strings before validation.</para>
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class UniqueStringsAttribute : ValidationAttribute
{
	/// <inheritdoc/>
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if (value is not List<string> items)
			return new ValidationResult("Invalid string list.");

		List<string> cleaned = [.. items
			.Where(t => !string.IsNullOrWhiteSpace(t))
			.Select(t => t.Trim())];

		if (cleaned.Distinct(StringComparer.OrdinalIgnoreCase).Count() != cleaned.Count)
			return new ValidationResult(ErrorMessage ?? "List items must be unique.");

		return ValidationResult.Success;
	}
}
