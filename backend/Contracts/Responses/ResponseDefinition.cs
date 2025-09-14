namespace JobifyEcom.Contracts.Responses;

/// <summary>
/// Base outcome model for both errors and successes.
/// </summary>
/// <param name="Id">Unique machine-readable code (e.g. <c>"AUTH_INVALID_SESSION"</c> or <c>"ORDER_PLACED"</c>).</param>
/// <param name="Title">Short, human-readable message.</param>
/// <param name="Details">Optional list of additional context.</param>
public abstract record ResponseDefinition(string Id, string Title, string[] Details)
{
	/// <summary>
	/// Returns a copy with <paramref name="title"/> replacing the existing <see cref="Title"/>.
	/// </summary>
	public ResponseDefinition WithTitle(string title) => this with { Title = title };

	/// <summary>
	/// Returns a copy with <paramref name="suffix"/> appended to the existing <see cref="Title"/>.
	/// </summary>
	public ResponseDefinition AppendTitle(string suffix) => this with { Title = $"{Title}{suffix}" };

	/// <summary>
	/// Returns a copy with <paramref name="details"/> replacing existing <see cref="Details"/>.
	/// </summary>
	public ResponseDefinition WithDetails(params string[] details) => this with { Details = details };

	/// <summary>
	/// Returns a copy with <paramref name="additionalDetails"/> appended to <see cref="Details"/>.
	/// </summary>
	public ResponseDefinition AppendDetails(params string[] additionalDetails)
		=> this with { Details = [.. Details, .. additionalDetails] };
}
