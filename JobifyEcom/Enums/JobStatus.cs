using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Enums;

/// <summary>
/// Represents the current status of a job within the platform.
/// </summary>
public enum JobStatus
{
	/// <summary>
	/// The job is open and available for workers to accept.
	/// </summary>
	[Display(Name = "Available")]
	Available,

	/// <summary>
	/// The job has been taken by a worker and is in progress.
	/// </summary>
	[Display(Name = "Booked")]
	Booked,

	/// <summary>
	/// The job has been completed by the assigned worker.
	/// </summary>
	[Display(Name = "Completed")]
	Completed,
}
