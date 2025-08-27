using JobifyEcom.Enums;

namespace JobifyEcom.DTOs.Job;

/// <summary>
/// Represents the data returned to clients when a job is retrieved or created.
/// </summary>
public class JobResponse
{
	/// <summary>
	/// The unique identifier of the job.
	/// </summary>
	public required Guid Id { get; set; }

	/// <summary>
	/// The title of the job post.
	/// </summary>
	public required string Title { get; set; }

	/// <summary>
	/// The detailed description of the job.
	/// </summary>
	public required string Description { get; set; }

	/// <summary>
	/// The price offered for completing the job.
	/// </summary>
	public required decimal Price { get; set; }

	/// <summary>
	/// The current status of the job (e.g., Open, InProgress, Completed).
	/// </summary>
	public required JobStatus Status { get; set; }

	/// <summary>
	/// The UTC timestamp when this job was created.
	/// </summary>
	public required DateTime CreatedAt { get; set; }

	/// <summary>
	/// The ID of the user who posted this job.
	/// </summary>
	public required Guid PostedByUserId { get; set; }
}
