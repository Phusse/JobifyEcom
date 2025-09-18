using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Validation;

/// <summary>
/// Validates that a <see cref="List{String}"/> does not exceed a specified maximum number of items.
/// <para>Use this attribute to enforce limits on collections of strings, such as tags, keywords, or other string lists.</para>
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="MaxListCountAttribute"/> with the specified maximum count.
/// </remarks>
/// <param name="maxCount">The maximum allowed number of items in the list.</param>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class MaxListCountAttribute(int maxCount) : ValidationAttribute
{
	/// <summary>
	/// Gets the maximum allowed number of items in the list.
	/// </summary>
	public int MaxCount { get; } = maxCount;

	/// <inheritdoc/>
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if (value is not List<string> items)
			return new ValidationResult("Invalid string list.");

		if (items.Count > MaxCount)
			return new ValidationResult(ErrorMessage ?? $"List cannot contain more than {MaxCount} items.");

		return ValidationResult.Success;
	}
}
