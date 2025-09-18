using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Validation;

/// <summary>
/// Validates that each string in a <see cref="List{String}"/> meets the specified minimum and maximum length.
/// <para>Before validation, each string is trimmed and empty or null values are ignored.</para>
/// <para>Use this attribute for validating collections of strings, such as tags, keywords, or names.</para>
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="StringLengthListAttribute"/> with the specified minimum and maximum lengths.
/// </remarks>
/// <param name="min">The minimum allowed length for each string.</param>
/// <param name="max">The maximum allowed length for each string.</param>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class StringLengthListAttribute(int min, int max) : ValidationAttribute
{
	/// <summary>
	/// Minimum allowed length for each string in the list.
	/// </summary>
	public int Min { get; } = min;

	/// <summary>
	/// Maximum allowed length for each string in the list.
	/// </summary>
	public int Max { get; } = max;

	/// <inheritdoc/>
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if (value is not List<string> items)
			return new ValidationResult("Invalid string list.");

		foreach (string item in items.Where(t => !string.IsNullOrWhiteSpace(t)))
		{
			if (item.Length < Min || item.Length > Max)
			{
				return new ValidationResult(ErrorMessage ??
					$"Each item must be between {Min} and {Max} characters. Invalid value: '{item}'"
				);
			}
		}

		return ValidationResult.Success;
	}
}
