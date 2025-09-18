using JobifyEcom.Contracts.Responses;

namespace JobifyEcom.DTOs;

/// <summary>
/// Represents the standardized result of a successful service operation.
/// This wrapper provides both the returned data (if any) and a strongly-typed,
/// catalogued response message for consistent API behavior.
///
/// <para><b>Note:</b> This class is not intended to represent failures —
/// throw exceptions for error scenarios. Use the <see cref="Create"/> factory
/// method to enforce the use of a <see cref="ResultResponseDefinition"/> so that
/// all messages come from the central success catalog.</para>
/// </summary>
/// <typeparam name="T">The type of data returned by the service operation.</typeparam>
public class ServiceResult<T>
{
	/// <summary>
	/// Unique machine-readable identifier for the message.
	/// </summary>
	public string? MessageId { get; set; }

	/// <summary>
	/// Human-readable summary message describing the result.
	/// </summary>
	public string? Message { get; set; }

	/// <summary>
	/// Additional context or details about the operation.
	/// Typically contains non-critical information about the result.
	/// </summary>
	public List<string>? Details { get; set; }

	/// <summary>
	/// The data returned by the service. May be null if the operation
	/// did not produce a value (e.g. a delete operation).
	/// </summary>
	public T? Data { get; set; }

	/// <summary>
	/// Creates a new <see cref="ServiceResult{T}"/> using a standardized
	/// <see cref="ResultResponseDefinition"/> for the message and details.
	/// This enforces consistent response payloads across the application.
	/// </summary>
	/// <param name="result">The standardized result definition to use.</param>
	/// <param name="data">Optional data to include in the result.</param>
	/// <returns>A fully populated <see cref="ServiceResult{T}"/>.</returns>
	public static ServiceResult<T> Create(ResultResponseDefinition result, T? data = default) => new()
	{
		MessageId = result.Id,
		Data = data,
		Message = result.Title,
		Details = [.. result.Details],
	};
}
