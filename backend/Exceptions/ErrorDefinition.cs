namespace JobifyEcom.Exceptions;

/// <summary>
/// Standardized error model for consistent API responses.
/// </summary>
/// <param name="Code">Unique machine-readable error code (e.g. <c>"AUTH_INVALID_SESSION"</c>).</param>
/// <param name="HttpStatus">HTTP status code to return (e.g. 401, 409).</param>
/// <param name="Title">Short, human-readable error message.</param>
/// <param name="Details">Optional list of additional context, such as validation messages.</param>
public record ErrorDefinition(
	string Code,
	int HttpStatus,
	string Title,
	string[] Details
)
{
	/// <summary>
	/// Returns a copy with <paramref name="details"/> replacing existing <see cref="Details"/>.
	/// </summary>
	public ErrorDefinition WithDetails(params string[] details)
		=> this with { Details = details };

	/// <summary>
	/// Returns a copy with <paramref name="additionalDetails"/> appended to <see cref="Details"/>.
	/// </summary>
	public ErrorDefinition AppendDetails(params string[] additionalDetails)
		=> this with { Details = Details.Concat(additionalDetails).ToArray() };
}
