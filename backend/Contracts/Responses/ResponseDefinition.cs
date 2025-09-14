namespace JobifyEcom.Contracts.Responses;

/// <summary>
/// Base outcome model for both errors and successes.
/// </summary>
/// <typeparam name="TSelf">The concrete type inheriting from this record.</typeparam>
/// <param name="Id">Unique machine-readable code (e.g. <c>"AUTH_INVALID_SESSION"</c> or <c>"ORDER_PLACED"</c>).</param>
/// <param name="Title">Short, human-readable message.</param>
/// <param name="Details">Optional list of additional context.</param>
public abstract record ResponseDefinition<TSelf>(string Id, string Title, string[] Details) where TSelf : ResponseDefinition<TSelf>
{
	/// <summary>
	/// Returns the current instance cast to the concrete derived type <typeparamref name="TSelf"/>.
	/// This allows fluent methods in the base record to return the correct derived type
	/// instead of the base type.
	/// </summary>
	protected TSelf Self => (TSelf)this;

	/// <summary>
	/// Returns a copy with <paramref name="title"/> replacing the existing <see cref="Title"/>.
	/// </summary>
	public TSelf WithTitle(string title) => Self with { Title = title };

	/// <summary>
	/// Returns a copy with <paramref name="suffix"/> appended to the existing <see cref="Title"/>.
	/// </summary>
	public TSelf AppendTitle(string suffix) => Self with { Title = $"{Title}{suffix}" };

	/// <summary>
	/// Returns a copy with <paramref name="details"/> replacing existing <see cref="Details"/>.
	/// </summary>
	public TSelf WithDetails(params string[] details) => Self with { Details = details };

	/// <summary>
	/// Returns a copy with <paramref name="additionalDetails"/> appended to <see cref="Details"/>.
	/// </summary>
	public TSelf AppendDetails(params string[] additionalDetails)
		=> Self with { Details = [.. Details, .. additionalDetails] };
}