namespace JobifyEcom.Helpers;

/// <summary>
/// Provides helper methods for performing conditional updates on mutable objects.
/// </summary>
public static class UpdateHelper
{
	/// <summary>
	/// Conditionally updates a value if the provided <paramref name="newValue"/> is not null.
	/// </summary>
	/// <typeparam name="T">The type of the value being updated.</typeparam>
	/// <param name="newValue">The new value to apply if not null.</param>
	/// <param name="updateAction">The action that applies the update to the target object.</param>
	/// <returns><c>true</c> if the value was updated; otherwise <c>false</c>.</returns>
	public static bool TryUpdate<T>(T? newValue, Action<T> updateAction)
	{
		if (newValue is not null)
		{
			updateAction(newValue);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Conditionally updates a value type if <paramref name="newValue"/> has a value.
	/// </summary>
	public static bool TryUpdate<T>(T? newValue, Action<T> updateAction) where T : struct
	{
		if (newValue.HasValue)
		{
			updateAction(newValue.Value);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Conditionally updates a string value if it is not null, empty, or whitespace.
	/// </summary>
	public static bool TryUpdate(string? newValue, Action<string> updateAction)
	{
		if (!string.IsNullOrWhiteSpace(newValue))
		{
			updateAction(newValue.Trim());
			return true;
		}

		return false;
	}
}
