using JobifyEcom.Enums;

namespace JobifyEcom.DTOs.Job;

/// <summary>
/// Response DTO for a job application.
/// Includes core identifiers plus convenience info
/// about the job and worker for display purposes.
/// </summary>
public class JobApplicationResponse
{
	/// <summary>
	/// Unique ID of the application.
	/// </summary>
	public required Guid Id { get; set; }

	/// <summary>
	/// Current status of the application.
	/// </summary>
	public required JobApplicationStatus Status { get; set; }

	/// <summary>
	/// UTC date when the application was submitted.
	/// </summary>
	public required DateTime DateRequested { get; set; }

	/// <summary>
	/// ID of the worker who submitted the application.
	/// </summary>
	public required Guid WorkerId { get; set; }

	/// <summary>
	/// Display name of the worker (convenience).
	/// </summary>
	public required string WorkerName { get; set; }

	/// <summary>
	/// ID of the job this application belongs to.
	/// </summary>
	public required Guid JobPostId { get; set; }

	/// <summary>
	/// Title of the job being applied for (convenience).
	/// </summary>
	public required string JobTitle { get; set; }

	/// <summary>
	/// Price offered for the job (convenience).
	/// </summary>
	public required decimal JobPrice { get; set; }
}
